namespace GroundsIce.Model.Entities.Validators.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;

#pragma warning disable SA1618
    /// Generic type parameters must be documented
    /// <summary>
    /// Коллекция валидаций (Validation<T>), содержит метод AssertAll(), валидирует каждый объект из списка
    /// на его валидаторе и сравнивает с ожидаемым резльтатом. Сравнение происходит с помощью Assert.AreEqual
    /// из состава NUnit.Framework. Если результат отличается от ожидаемого, то выводится сообщение (средствами
    /// NUnit) содержащее индекс валидации, ожидаемый результат валидации, полученный резльтат и описание валидации
    /// (если имеется)
    /// </summary>
    /// <typeparam name="T">Тип объекта</typeparam>
    public class ValidationsCollection<T> : List<Validation<T>>
#pragma warning restore SA1618 // Generic type parameters must be documented
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
