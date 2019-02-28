namespace GroundsIce.Model.Entities.Validators.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NUnit.Framework;

    public class ValidationsCollection<T> : List<Validation<T>>
    {
        public void AssertAll()
        {
            for (int i = 0; i < this.Count; ++i)
            {
                var validation = this[i];
                bool expected = validation.Result;
                bool isValid = validation.Validator.Validate(validation.Object).IsValid;
                string assertMessage = $"Validation failed at index: {i}, expected: {expected}, but was: {isValid}";
                if (validation.Description != null)
                {
                    assertMessage += $", description: {validation.Description}";
                }

                Assert.AreEqual(expected, isValid, assertMessage);
            }
        }
    }
}
