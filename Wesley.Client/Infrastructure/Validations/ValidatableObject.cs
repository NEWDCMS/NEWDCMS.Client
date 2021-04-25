using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Wesley.Client.Validations
{
    public class ValidatableObject<T> : Base, IValidity
    {

        [Reactive]
        public ObservableCollection<string> Errors
        { get; set; }


        [Reactive]
        public List<IValidationRule<T>> Validations
        { get; set; }

        [Reactive]
        public T Value
        { get; set; }

        [Reactive]
        public bool IsValid
        { get; set; }

        public ValidatableObject()
        {
            IsValid = true;
            Errors = new ObservableCollection<string>();
            Validations = new List<IValidationRule<T>>();
        }

        public bool Validate()
        {
            Errors.Clear();

            IEnumerable<string> errors = Validations.Where(v => !v.Check(Value))
                                                     .Select(v => v.ValidationMessage);

            foreach (var error in errors)
            {
                Errors.Add(error);
            }

            IsValid = !Errors.Any();

            return IsValid;
        }
    }
}
