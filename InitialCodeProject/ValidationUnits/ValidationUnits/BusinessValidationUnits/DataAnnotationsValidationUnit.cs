using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ValidationUnits.BusinessValidationUnits
{
    public class DataAnnotationsValidationUnit<X> : IBusinessValidationUnit<X> where X : class
    {
        public bool IncludeChildren { get; set; }
        public DataAnnotationsValidationUnit(bool includeChildren = false)
        {
            this.IncludeChildren = includeChildren;
        }
        public ValidationUnitResult Validate(X model)
        {
            var result = EntityValidationHelper.ValidateEntity(model, this.IncludeChildren);

            return new ValidationUnitResult()
            {
                ErrorMessages = result.Errors.Select(x => x.ErrorResult.ErrorMessage).ToList()
            };
        }

        public static partial class EntityValidationHelper
        {
            private static Dictionary<string, AssociatedMetadataTypeTypeDescriptionProvider> _metaDataTypeList = new Dictionary<string, AssociatedMetadataTypeTypeDescriptionProvider>();

            /// <summary>
            /// Validates the entity.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="entity">The entity.</param>
            /// <returns></returns>
            public static EntityValidationResult ValidateEntity<T>(T entity, bool includeChildren = false) where T : class
            {
                Action<string, AssociatedMetadataTypeTypeDescriptionProvider> callBack = (key, value) =>
                {
                    if (!_metaDataTypeList.ContainsKey(key))
                    {
                        _metaDataTypeList.Add(key, value);
                    }
                };

                _metaDataTypeList.Clear();
                return new EntityValidator<T>(callBack, _metaDataTypeList).Validate(entity, includeChildren);
            }

            /// <summary>
            /// Validates the entity.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="entity">The entity.</param>
            /// <param name="includeChildren">if set to <c>true</c> [include children].</param>
            /// <returns></returns>
            public static EntityValidationResult ValidateEntity<T>(List<T> entity, bool includeChildren = false) where T : class
            {
                Action<string, AssociatedMetadataTypeTypeDescriptionProvider> callBack = (key, value) =>
                {
                    if (!_metaDataTypeList.ContainsKey(key))
                    {
                        _metaDataTypeList.Add(key, value);
                    }
                };

                _metaDataTypeList.Clear();
                return new EntityValidator<T>(callBack, _metaDataTypeList).Validate(entity, includeChildren);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private interface IEntity
        {
            /// <summary>
            /// Gets or sets the id.
            /// </summary>
            /// <value>
            /// The id.
            /// </value>
            string HashedKey { get; set; }

            /// <summary>
            /// Validates me.
            /// </summary>
            /// <returns></returns>
            bool ValidateMe();

            /// <summary>
            /// Walks the object graph.
            /// </summary>
            /// <param name="entityExistCondition">Condition to exist the object graph walk when encountering a single entity.</param>
            /// <param name="listExistCondition">Condition to exist the object graph walk when encountering a list.</param>
            /// <param name="exemptProperties">The exempt properties.</param>
            void WalkObjectGraph(Func<IEntity, bool> entityExistCondition, Func<IEnumerable, bool> listExistCondition, params string[] exemptProperties);
        }

        /// <summary>
        /// Defines a Validator class.
        /// </summary>
        /// <typeparam name="T">The entity to validate.</typeparam>
        private interface IValidator<T> where T : class
        {
            /// <summary>
            /// Validates the specified entity.
            /// </summary>
            /// <param name="entity">The entity.</param>
            /// <returns>Returns a <see cref="EntityValidationResult"/>.</returns>
            EntityValidationResult Validate(T entity);
        }

        /// <summary>
        /// Class to help with validating objects using MetaData attributes.
        /// </summary>
        /// <typeparam name="T">The entity to validate.</typeparam>
        private class EntityValidator<T> : IValidator<T> where T : class
        {
            /// <summary>
            /// The _call back
            /// </summary>
            private Action<string, AssociatedMetadataTypeTypeDescriptionProvider> _callBack;

            /// <summary>
            /// The meta data type list
            /// </summary>
            private Dictionary<string, AssociatedMetadataTypeTypeDescriptionProvider> _metaDataTypeList = new Dictionary<string, AssociatedMetadataTypeTypeDescriptionProvider>();

            /// <summary>
            /// Initializes a new instance of the <see cref="EntityValidator{T}"/> class.
            /// </summary>
            /// <param name="callBack">The call back.</param>
            public EntityValidator(Action<string, AssociatedMetadataTypeTypeDescriptionProvider> callBack, Dictionary<string, AssociatedMetadataTypeTypeDescriptionProvider> metaDataTypeList)
            {
                _callBack = callBack;
                _metaDataTypeList = metaDataTypeList;
            }

            /// <summary>
            /// Validates the specified entity.
            /// </summary>
            /// <param name="entity">The entity.</param>
            /// <returns>Return <see cref="EntityValidationResult"/>.</returns>
            public EntityValidationResult Validate(T entity)
            {
                return ValidateEntity(typeof(T), entity);
            }

            /// <summary>
            /// Validates the specified entity.
            /// </summary>
            /// <param name="entity">The entity.</param>
            /// <param name="includeChildren">if set to <c>true</c> [include children].</param>
            /// <returns>
            /// Return <see cref="EntityValidationResult" />.
            /// </returns>
            public EntityValidationResult Validate(T entity, bool includeChildren, bool exitOnError = true)
            {
                var validationResult = ValidateEntity(typeof(T), entity);

                if (!validationResult.HasError && includeChildren && entity is IEntity)
                {
                    var tempEntity = entity as IEntity;

                    tempEntity.WalkObjectGraph(
                    item =>
                    {
                        var result = this.ValidateEntity(item.GetType(), item);

                        if (result.HasError)
                        {
                            foreach (var error in result.Errors)
                            {
                                validationResult.Errors.Add(error);
                            }

                            return exitOnError;
                        }

                        return false;
                    },
                    list =>
                    {
                        return false;
                    });
                }

                return validationResult;
            }

            /// <summary>
            /// Validates the specified entities.
            /// </summary>
            /// <param name="entities">The entities.</param>
            /// <param name="includeChildren">if set to <c>true</c> [include children].</param>
            /// <param name="exitOnError">if set to <c>true</c> [exit on error].</param>
            /// <returns></returns>
            public EntityValidationResult Validate(List<T> entities, bool includeChildren, bool exitOnError = true)
            {
                EntityValidationResult validationResult = null;

                foreach (T entity in entities)
                {
                    validationResult = Validate(entity, includeChildren, exitOnError);

                    if (validationResult.HasError)
                    {
                        return validationResult;
                    }
                }

                return validationResult;
            }

            /// <summary>
            /// Validates the specified child entity.
            /// </summary>
            /// <typeparam name="U">The type of the entity</typeparam>
            /// <param name="type">The type.</param>
            /// <param name="entity">The entity.</param>
            /// <returns>
            /// Return <see cref="EntityValidationResult" />.
            /// </returns>
            private EntityValidationResult ValidateEntity<U>(Type type, U entity)
            {
                if (!_metaDataTypeList.ContainsKey(type.Name))
                {
                    var metadataAttrib = entity.GetType().GetCustomAttributes(typeof(MetadataTypeAttribute), true).OfType<MetadataTypeAttribute>().FirstOrDefault();
                    System.Type buddyClassOrModelClass = metadataAttrib != null ? metadataAttrib.MetadataClassType : entity.GetType();

                    if (_callBack != null)
                    {
                        _callBack(type.Name, new AssociatedMetadataTypeTypeDescriptionProvider(type, buddyClassOrModelClass));
                    }
                }

                TypeDescriptor.AddProviderTransparent(_metaDataTypeList[type.Name], type);

                var validationResults = new List<ValidationResult>();
                var vc = new ValidationContext(entity, null, null);
                var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(entity, vc, validationResults, true);

                var errorList = validationResults.Select(r => new EntityValidationError { EntityType = type, ErrorResult = r }).ToList();

                return new EntityValidationResult(errorList);
            }
        }
    }

    public class EntityValidationError
    {
        public Type EntityType { get; set; }
        public ValidationResult ErrorResult { get; set; }

        public EntityValidationError()
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [DataContract(Name = "EntityValidationResult", Namespace = "http://glass_prototype.ucmerced.edu/types")]
    public class EntityValidationResult
    {
        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <value>
        /// The errors.
        /// </value>
        public IList<EntityValidationError> Errors
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has error.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has error; otherwise, <c>false</c>.
        /// </value>
        public bool HasError
        {
            get
            {
                return Errors.Count > 0;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityValidationResult"/> class.
        /// </summary>
        /// <param name="errors">The errors.</param>
        public EntityValidationResult(IList<EntityValidationError> errors = null)
        {
            Errors = errors ?? new List<EntityValidationError>();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> concatenation of all the error messages.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> concatenation of all the error messages.
        /// </returns>
        public override string ToString()
        {
            return string.Join(Environment.NewLine, Errors.Select(valError => valError.ErrorResult.ErrorMessage));
        }
    }
}
