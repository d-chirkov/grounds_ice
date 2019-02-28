namespace GroundsIce.Model.Entities.Validators.Tests
{
    using FluentValidation;

    /// <summary>
    /// Объект для тестирования валидации данных, содержит сам объект (Object), валидатор (Validator)
    /// и ожидаемый результат валидации объекта на валидаторе (Result)
    /// </summary>
    /// <typeparam name="T">Тип объекта</typeparam>
    public class Validation<T>
    {
        public T Object { get; set; }

        public IValidator<T> Validator { get; set; }

        public bool Result { get; set; }

        public string Description { get; set; }
    }
}
