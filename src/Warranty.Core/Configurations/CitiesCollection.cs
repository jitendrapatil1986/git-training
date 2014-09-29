using System;
using System.Configuration;

namespace Warranty.Core.Configurations
{
    public class CitiesCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return
                    ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new CityConfig();
        }

        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            var result = new CityConfig { City = elementName };

            return result;
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            var cityConfig = (CityConfig)element;
            return cityConfig.City;
        }

        public new string AddElementName
        {
            get
            { return base.AddElementName; }

            set
            { base.AddElementName = value; }

        }

        public new string ClearElementName
        {
            get
            { return base.ClearElementName; }

            set
            { base.AddElementName = value; }

        }

        public new string RemoveElementName
        {
            get
            { return base.RemoveElementName; }
        }

        public new int Count
        {
            get { return base.Count; }
        }

        public CityConfig this[int index]
        {
            get
            {
                return (CityConfig)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public CityConfig this[string Name]
        {
            get
            {
                return (CityConfig)BaseGet(Name);
            }
        }

        public int IndexOf(CityConfig mapping)
        {
            return BaseIndexOf(mapping);
        }

        public void Add(CityConfig mapping)
        {
            BaseAdd(mapping);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, true);
        }

        public void Remove(CityConfig mapping)
        {
            if (BaseIndexOf(mapping) >= 0)
                BaseRemove(mapping.City);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
        }

        public override bool IsReadOnly()
        {
            return false;
        }
    }
}