using FluentValidation.Internal;
using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;
using System.Reflection;

namespace Sincronizados.WEB.Validation
{
    static class EditContextExtensions
    {
        public static EditContext AddFluentValidation(this EditContext editContext)
        {
            if (editContext == null)
            {
                throw new ArgumentNullException(nameof(editContext));
            }

            var messages = new ValidationMessageStore(editContext);

            editContext.OnValidationRequested +=
                (sender, eventArgs) => ValidateModel((EditContext)sender, messages);

            editContext.OnFieldChanged +=
                (sender, eventArgs) => ValidateField(editContext, messages, eventArgs.FieldIdentifier);

            return editContext;
        }

        private static void ValidateModel(EditContext editContext, ValidationMessageStore messages)
        {
            var validator = GetValidatorForModel(editContext.Model);
            var context = CreateValidationContextForModel(editContext.Model);
            var validationResults = validator.Validate(context);
            messages.Clear();
            foreach (var validationResult in validationResults.Errors)
            {
                messages.Add(editContext.Field(validationResult.PropertyName), validationResult.ErrorMessage);
            }
            editContext.NotifyValidationStateChanged();
        }

        private static void ValidateField(EditContext editContext, ValidationMessageStore messages, in FieldIdentifier fieldIdentifier)
        {

            var properties = new[] { fieldIdentifier.FieldName };

            var context = CreateValidationContextForProperties(fieldIdentifier.Model, properties);
            var validator = GetValidatorForModel(fieldIdentifier.Model);
            var validationResults = validator.Validate(context);

            messages.Clear(fieldIdentifier);
            messages.Add(fieldIdentifier, validationResults.Errors.Select(error => error.ErrorMessage));
            editContext.NotifyValidationStateChanged();
        }

        private static IValidationContext CreateValidationContextForProperties(object model, string[] properties)
        {
            var valType = typeof(ValidationContext<>).MakeGenericType(model.GetType());
            return (IValidationContext)Activator.CreateInstance(valType, new[] { model, new PropertyChain(), new MemberNameValidatorSelector(properties) })!;
        }

        private static IValidationContext CreateValidationContextForModel(object model)
        {
            var valType = typeof(ValidationContext<>).MakeGenericType(model.GetType());
            return (IValidationContext)Activator.CreateInstance(valType, new[] { model });
        }

        private static IValidator GetValidatorForModel(object model)
        {
            var abstractValidatorType = typeof(AbstractValidator<>).MakeGenericType(model.GetType());
            var modelValidatorType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.IsSubclassOf(abstractValidatorType));
            var modelValidatorInstance = (IValidator)Activator.CreateInstance(modelValidatorType)!;
            return modelValidatorInstance;
        }
    }

}
