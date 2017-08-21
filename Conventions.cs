using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
 

namespace ConventionUtils
{
    public class DateTime2Convention : Convention
    {
        //PB Continually wonder at the awesomeness of this 
        public DateTime2Convention()
        {
            Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));

        }
    }

    public class NormalStringLength : Convention
    {
        public NormalStringLength()
        {
            Properties<string>().Configure(c => c.HasMaxLength(300));
        }
    }

    public class MakeMagicIDConvention : IStoreModelConvention<EntitySet>, IStoreModelConvention<EdmProperty>
    {

        public void Apply(EntitySet item, DbModel model)
        {


            //if (!item.Name.StartsWith("T_"))
            //    item.Name = "T_" + item.Name;
            //   item.Name = item.Name.ToUpper();
        }

        public void Apply(EdmProperty item, DbModel model)
        {
            if (item.Name == "__ID__")
            {
                item.Name = item.DeclaringType.Name + "ID";
            }

        }
    }





    public sealed class DiscriminatorConvention : TypeAttributeConfigurationConvention<RecordTypeAttribute>
    {
        private static readonly MethodInfo EntityMethod = typeof(DbModelBuilder).GetMethod("Entity");
        private static readonly MethodInfo HasValueMethod = typeof(ValueConditionConfiguration).GetMethods().Single(m => (m.Name == "HasValue") && (m.IsGenericMethod == false));

        private readonly DbModelBuilder _modelBuilder;
        private readonly ISet<Type> _types = new HashSet<Type>();

        public DiscriminatorConvention(DbModelBuilder modelBuilder)
        {
            _modelBuilder = modelBuilder;
        }

        public override void Apply(ConventionTypeConfiguration configuration, RecordTypeAttribute attribute)
        {
            if (_types.Contains(configuration.ClrType))
            {
                //if the type has already been processed, bail out
                return;
            }

            //add the type to the list of processed types
            _types.Add(configuration.ClrType);

            dynamic entity = EntityMethod.MakeGenericMethod(configuration.ClrType).Invoke(_modelBuilder, null);

            Action<dynamic> action = arg =>
             {
                 var valueConditionConfiguration = arg.Requires("Discriminator");
                 HasValueMethod.Invoke(valueConditionConfiguration, new object[] { attribute.RecordTypeCode.ToString() });
             };

            entity.Map(action);
        }
    }


    /// <summary>
    /// EF doesn't support a dataannotation method of definining cascade deletes, and I don't like the Fluent API, so here is a model convention which processes
    /// the above attribute
    /// </summary>
    public class CascadeDeleteAttributeConvention : IConceptualModelConvention<AssociationType>
    {
        private static readonly Func<AssociationType, bool> IsSelfReferencing;
        private static readonly Func<AssociationType, bool> IsRequiredToMany;
        private static readonly Func<AssociationType, bool> IsManyToRequired;
        private static readonly Func<AssociationType, object> GetConfiguration;
        private static readonly Func<object, OperationAction?> NavigationPropertyConfigurationGetDeleteAction;

        static CascadeDeleteAttributeConvention()
        {
            var associationTypeExtensionsType = typeof(AssociationType).Assembly.GetType("System.Data.Entity.ModelConfiguration.Edm.AssociationTypeExtensions");
            var navigationPropertyConfigurationType = typeof(AssociationType).Assembly.GetType("System.Data.Entity.ModelConfiguration.Configuration.Properties.Navigation.NavigationPropertyConfiguration");

            var isSelfRefencingMethod = associationTypeExtensionsType.GetMethod("IsSelfReferencing", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            IsSelfReferencing = associationType => (bool)isSelfRefencingMethod.Invoke(null, new object[] { associationType });

            var isRequiredToManyMethod = associationTypeExtensionsType.GetMethod("IsRequiredToMany", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            IsRequiredToMany = associationType => (bool)isRequiredToManyMethod.Invoke(null, new object[] { associationType });

            var isManyToRequiredMethod = associationTypeExtensionsType.GetMethod("IsManyToRequired", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            IsManyToRequired = associationType => (bool)isManyToRequiredMethod.Invoke(null, new object[] { associationType });

            var getConfigurationMethod = associationTypeExtensionsType.GetMethod("GetConfiguration", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            GetConfiguration = associationType => getConfigurationMethod.Invoke(null, new object[] { associationType });

            var deleteActionProperty = navigationPropertyConfigurationType.GetProperty("DeleteAction", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            NavigationPropertyConfigurationGetDeleteAction = navProperty => (OperationAction?)deleteActionProperty.GetValue(navProperty);
        }

        public virtual void Apply(AssociationType item, DbModel model)
        {

            if (IsSelfReferencing(item))
            {
                //  Debug.Print("{0} is self referencing", item.FullName);
                //         return;
            }
            var propertyConfiguration = GetConfiguration(item);
            if (propertyConfiguration != null && NavigationPropertyConfigurationGetDeleteAction(propertyConfiguration).HasValue)
            {
//                Debug.Print("{0} already has delete action", item.FullName);
                return;
            }

            AssociationEndMember collectionEndMember = null;
            AssociationEndMember singleNavigationEndMember = null;
            if (IsRequiredToMany(item))
            {
                collectionEndMember = GetSourceEnd(item);
                singleNavigationEndMember = GetTargetEnd(item);
            }
            else if (IsManyToRequired(item))
            {
                collectionEndMember = GetTargetEnd(item);
                singleNavigationEndMember = GetSourceEnd(item);
            }
            if (collectionEndMember == null || singleNavigationEndMember == null)
            {
                // Debug.Print("{0} can't work out 1 of the ends", item.FullName);

                return;
            }

            var iscollectionCascadeDeleteAttribute = GetCascadeDeleteAttribute(collectionEndMember);
            var issingleCascadeDeleteAttribute = GetCascadeDeleteAttribute(singleNavigationEndMember);

            if (iscollectionCascadeDeleteAttribute || issingleCascadeDeleteAttribute)
            {
                //#if SHOWCONVENTIONS
                Debug.Print("Applying CDA convention to relationship {0} {1}", singleNavigationEndMember.Name, collectionEndMember.Name);
                //#endif
                if (iscollectionCascadeDeleteAttribute) collectionEndMember.DeleteBehavior = OperationAction.Cascade;
                if (issingleCascadeDeleteAttribute) singleNavigationEndMember.DeleteBehavior = OperationAction.Cascade;

            }
            else
            {

                //          Debug.Print("Cannot apply CDA convention to relationship {0} {1}", singleNavigationEndMember.Name, collectionEndMember.Name);
            }
        }

        private static AssociationEndMember GetSourceEnd(EntityTypeBase item)
        {
            return item.KeyMembers.FirstOrDefault() as AssociationEndMember;
        }
        private static AssociationEndMember GetTargetEnd(EntityTypeBase item)
        {
            return item.KeyMembers.ElementAtOrDefault(1) as AssociationEndMember;
        }

        private static bool GetCascadeDeleteAttribute(EdmMember edmMember)
        {
            var clrProperties = edmMember.MetadataProperties.FirstOrDefault(m => m.Name == "ClrPropertyInfo");

            var property = clrProperties?.Value as PropertyInfo;
            if (property == null)

            {
                return false;
            }
            try
            {
                return property.CustomAttributes.Any(p => p.AttributeType == typeof(CascadeDeleteAttribute));
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
            return false;
        }
    }
}

