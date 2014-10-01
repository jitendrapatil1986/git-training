namespace Warranty.UI.Core.ModelBinders
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using Helpers;
    using Warranty.Core.FileManagement;
    using Warranty.Core.Services;
    using Yay.Enumerations;
    using Yay.Enumerations.MVC.ModelBinders;


    public class FileUploadsPropertyBinder : FilteredPropertyBinderBase
    {
        public const string FileUploaderIdFieldName = "FileUploaderId";

        private readonly TemporaryFileUploadsService _temporaryFileUploadsService;

        public FileUploadsPropertyBinder(TemporaryFileUploadsService temporaryFileUploadsService)
        {
            _temporaryFileUploadsService = temporaryFileUploadsService;
        }

        public override bool ShouldBind(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            return propertyDescriptor.PropertyType.IsAssignableTo<FileUpload>();
        }

        public override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            try
            {
                var uploaderId = GetChildPropertyAttemptedValue(bindingContext, propertyDescriptor, FileUploaderIdFieldName);
                var rawNewUploads = GetChildPropertyAttemptedValue(bindingContext, propertyDescriptor, uploaderId);
                var rawPendingUploads = GetChildPropertyAttemptedValue(bindingContext, propertyDescriptor, x => x.RawPendingUploads);

                var allPendingUploads = JoinAllUploads(rawPendingUploads, rawNewUploads);

                var fileGuids = GetFileGuids(allPendingUploads);

                var temporaryFileUploadInfos = new List<TemporaryFileUploadInfo>();
                foreach (var fileGuid in fileGuids)
                {
                    var temporaryFileUploadInfo = new TemporaryFileUploadInfo
                    {
                        FileId = fileGuid,
                        FileName = _temporaryFileUploadsService.GetOriginalFileName(fileGuid),
                    };

                    temporaryFileUploadInfos.Add(temporaryFileUploadInfo);
                }

                var previousFiles = ReHydrateContractFileInfos(bindingContext, propertyDescriptor);

                var result = new FileUpload
                {
                    RawPendingUploads = allPendingUploads,
                    PendingFiles = temporaryFileUploadInfos,
                    PendingFileIds = fileGuids,
                    PreviousFiles = previousFiles.ToArray(),
                };

                PerformModelValidation(bindingContext, propertyDescriptor, result);

                return result;
            }
            catch (Exception ex)
            {
                var message = string.Format("Unable to locate a valid value for '{0}'", GetRootPropertyName(bindingContext, propertyDescriptor));
                throw new ApplicationException(message, ex);
            }
        }

        private static List<string> ReHydrateContractFileInfos(ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            var rootPropertyName = GetRootPropertyName(bindingContext, propertyDescriptor);

            var previousFiles = new List<string>();
            for (var i = 0; i < 1000; i++)
            {
                var rootPropertyPreviousFileN = string.Format("{0}.{1}[{2}]",
                    rootPropertyName,
                    ReflectionService.GetPropertyName<FileUpload>(x => x.PreviousFiles),
                    i);

                var fileIdValueProviderResult = bindingContext.ValueProvider.GetValue(rootPropertyPreviousFileN);
                if (fileIdValueProviderResult == null)
                    break;

                var fileName = bindingContext.ValueProvider.GetValue(rootPropertyPreviousFileN).AttemptedValue;

                previousFiles.Add(fileName);
            }
            return previousFiles;
        }

        private static void PerformModelValidation(ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, FileUpload result)
        {
            var attributes = propertyDescriptor.Attributes;
            if (attributes.OfType<RequiredAttribute>().Any() && !result.PendingFileIds.Any())
            {
                var requiredAttribute = attributes.OfType<RequiredAttribute>().First();
                bindingContext.ModelState.AddModelError(GetRootPropertyName(bindingContext, propertyDescriptor), requiredAttribute.ErrorMessage);
            }
        }

        private static string JoinAllUploads(string rawPreviousUploads, string rawNewUploads)
        {
            if (string.IsNullOrWhiteSpace(rawPreviousUploads))
                return rawNewUploads;

            if (string.IsNullOrWhiteSpace(rawNewUploads))
                return rawPreviousUploads;

            return rawPreviousUploads + "/" + rawNewUploads;
        }

        private static List<string> GetFileGuids(string raw)
        {
            return string.IsNullOrWhiteSpace(raw) ? new List<string>() : raw.Split('/').ToList();
        }

        protected static string GetChildPropertyAttemptedValue(ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, Expression<Func<FileUpload, object>> expression)
        {
            var propertyName = ReflectionService.GetPropertyName(expression);

            return GetChildPropertyAttemptedValue(bindingContext, propertyDescriptor, propertyName);
        }

        protected static string GetChildPropertyAttemptedValue(ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, string propertyName)
        {
            var rootPropertyName = GetRootPropertyName(bindingContext, propertyDescriptor);

            var valueProviderResult = bindingContext.ValueProvider.GetValue(rootPropertyName + "." + propertyName);

            if (valueProviderResult == null && PropertyIsInACollection(rootPropertyName))
                valueProviderResult = bindingContext.ValueProvider.GetValue(ModelBindingHelper.ReplaceSpecialChactersWithDotReplacement(rootPropertyName) + "." + propertyName);

            return valueProviderResult != null ? valueProviderResult.AttemptedValue : null;
        }

        private static bool PropertyIsInACollection(string rootPropertyName)
        {
            var isCollection = new System.Text.RegularExpressions.Regex(@"\[\w\]");
            return isCollection.IsMatch(rootPropertyName);
        }

        private static string GetRootPropertyName(ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            var prefix = "";

            if (!string.IsNullOrWhiteSpace(bindingContext.ModelName))
                prefix = bindingContext.ModelName + ".";

            var rootPropertyName = prefix + propertyDescriptor.Name;
            return rootPropertyName;
        }
    }
}