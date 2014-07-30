using System;
using System.Linq;
using System.Reflection;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;
using Warranty.Core.Extensions;
using NHibernate;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;

namespace Warranty.Core.DataAccess
{
    public class HbmMappingGenerator
    {
        public HbmMapping GenerateMappings()
        {
            var mapper = new ModelMapper();

            mapper.BeforeMapClass += (inspector, member, customizer) => customizer.Id(m => m.Generator(Generators.Identity));
            mapper.BeforeMapProperty += MapperOnBeforeMapProperty;
            mapper.BeforeMapManyToOne += (inspector, member, customizer) =>
            {
                customizer.Column(member.LocalMember.Name + "Id");
                var containerType = member.GetContainerEntity(inspector);
                customizer.ForeignKey("FK_" + containerType.Name + "_" + member.LocalMember.GetPropertyOrFieldType().Name + "_" + member.LocalMember.Name);
                customizer.Index("IX_" + containerType.Name + "_" + member.LocalMember.GetPropertyOrFieldType().Name);
            };
            mapper.BeforeMapList += (inspector, member, customizer) =>
            {
                customizer.Access(Accessor.Field);
                customizer.Cascade(Cascade.Persist);
                customizer.Key(m =>
                {
                    var containerType = member.GetContainerEntity(inspector);
                    m.ForeignKey("FK_" + member.CollectionElementType().Name + "_" + containerType.Name + "_" + member.LocalMember.Name);
                    m.Column(cm =>
                    {
                        cm.Name(DetermineKeyColumnName(inspector, member));
                        cm.Index("IX_" + member.CollectionElementType().Name + "_" + containerType.Name);
                    });
                });
            };
            mapper.BeforeMapSet += (inspector, member, customizer) =>
            {
                customizer.Access(Accessor.Field);
                customizer.Cascade(Cascade.Persist);
                customizer.Key(m =>
                {
                    var containerType = member.GetContainerEntity(inspector);
                    m.ForeignKey("FK_" + member.CollectionElementType().Name + "_" + containerType.Name + "_" + member.LocalMember.Name);
                    m.Column(cm =>
                    {
                        cm.Name(DetermineKeyColumnName(inspector, member));
                        cm.Index("IX_" + member.CollectionElementType().Name + "_" + containerType.Name);
                    });
                });
            };
            mapper.BeforeMapOneToMany += (inspector, member, customizer) => customizer.Class(member.LocalMember.GetPropertyOrFieldType().GetGenericArguments().Single());
            mapper.BeforeMapManyToMany += (inspector, member, customizer) =>
            {
                var containerType = member.GetContainerEntity(inspector);
                customizer.ForeignKey("FK_" + containerType.Name + "_" + member.CollectionElementType().Name + "_" + member.LocalMember.Name);
            };

            mapper.AddMappings(Assembly.GetAssembly(typeof(Home)).GetExportedTypes());
            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
            return mapping;
        }

        private void MapperOnBeforeMapProperty(IModelInspector modelInspector, PropertyPath member, IPropertyMapper propertyCustomizer)
        {
            var memberType = member.LocalMember.GetPropertyOrFieldType();

            var rootColumnName = member.LocalMember.Name;
            var isComponent = modelInspector.IsComponent(member.LocalMember.DeclaringType);

            if (isComponent)
            {
                rootColumnName = member.PreviousPath.LocalMember.Name + rootColumnName;
            }

            if (memberType.Closes(typeof(Int32Enumeration<>)))
            {
                propertyCustomizer.Type(typeof(Int32EnumerationType<>).MakeGenericType(memberType), null);
                propertyCustomizer.Columns(
                    c => { c.Name(rootColumnName + "Value"); c.SqlType("INT"); },
                    c => { c.Name(rootColumnName + "DisplayName"); c.SqlType("VARCHAR(8000)"); });
            }
            else if (memberType.Closes(typeof(StringEnumeration<>)))
            {
                propertyCustomizer.Type(typeof(StringEnumerationType<>).MakeGenericType(memberType), null);
                propertyCustomizer.Columns(
                    c => { c.Name(rootColumnName + "Value"); c.SqlType("VARCHAR(8000)"); },
                    c => { c.Name(rootColumnName + "DisplayName"); c.SqlType("VARCHAR(8000)"); }
                    );
            }
            else if (memberType.Closes(typeof(DateTime)))
            {
                propertyCustomizer.Type(typeof(DateTime).MakeGenericType(memberType), null);
                propertyCustomizer.Columns(c => { c.Name(rootColumnName); c.SqlType("DATETIME2"); });
            }
            else if (memberType == typeof(string))
            {
                propertyCustomizer.Type(NHibernateUtil.AnsiString);
            }
            else if (memberType == typeof(DateTime) || memberType == typeof(DateTime?))
            {
                propertyCustomizer.Type(NHibernateUtil.DateTime2);
            }
            else if (isComponent)
            {
                propertyCustomizer.Column(rootColumnName);
            }
        }

        private static string DetermineKeyColumnName(IModelInspector inspector, PropertyPath member)
        {
            var otherSideProperty = member.OneToManyOtherSideProperty();
            if (inspector.IsOneToMany(member.LocalMember) && otherSideProperty != null)
                return otherSideProperty.Name + "Id";

            return member.Owner().Name + "Id";
        }
    }
}