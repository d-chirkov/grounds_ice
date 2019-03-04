namespace GroundsIce.Model.Entities
{
    using System.Collections;
    using System.Collections.Generic;

    // TODO: complete tests
    public class ProfileEntriesCollection : ICollection<ProfileEntry>
    {
        private ICollection<ProfileEntry> innerCollection;

#pragma warning disable SA1611 // Element parameters must be documented
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileEntriesCollection"/> class.
        /// Конструктор, может либо не принимать аргментов, тогда в качестве внутренней
        /// коллекции будет использоваться List<ProfileEntry>, либо принимает коллекцию
        /// записей профиля ICollection<ProfileEntry>, в качестве внутренней коллекции используется
        /// ссылка на переданную конструктору коллекция (то есть никаких копий не делается)
        /// </summary>
        /// <param name="innerCollection">Внутренняя коллекция</param>
        public ProfileEntriesCollection(ICollection<ProfileEntry> innerCollection = null)
#pragma warning restore SA1611 // Element parameters must be documented
        {
            this.innerCollection = innerCollection ?? new List<ProfileEntry>();
        }

        public int Count => this.innerCollection.Count;

        public bool IsReadOnly => this.innerCollection.IsReadOnly;

        public IEnumerator<ProfileEntry> GetEnumerator() => this.innerCollection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.innerCollection.GetEnumerator();

        public void Add(ProfileEntry item) => this.innerCollection.Add(item);

        public void Clear() => this.innerCollection.Clear();

        public bool Contains(ProfileEntry item) => this.innerCollection.Contains(item);

        public void CopyTo(ProfileEntry[] array, int arrayIndex) => this.innerCollection.CopyTo(array, arrayIndex);

        public bool Remove(ProfileEntry item) => this.innerCollection.Remove(item);
    }
}
