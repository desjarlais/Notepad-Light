using System.Collections;

namespace Notepad_Light.NetSpeller.Phonetic
{
    public class PhoneticRuleEnumerator : object, IEnumerator
    {
        private IEnumerator Base;

        private IEnumerable Local;

        /// <summary>
        ///     Enumerator constructor
        /// </summary>
        public PhoneticRuleEnumerator(PhoneticRuleCollection mappings)
        {
            this.Local = ((IEnumerable)(mappings));
            this.Base = Local.GetEnumerator();
        }

        /// <summary>
        ///     Gets the current element from the collection (strongly typed)
        /// </summary>
        public PhoneticRule Current
        {
            get
            {
                return ((PhoneticRule)(Base.Current));
            }
        }

        /// <summary>
        ///     Gets the current element from the collection
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                return Base.Current;
            }
        }

        /// <summary>
        ///     Advances the enumerator to the next element of the collection
        /// </summary>
        public bool MoveNext()
        {
            return Base.MoveNext();
        }

        /// <summary>
        ///     Advances the enumerator to the next element of the collection
        /// </summary>
        bool IEnumerator.MoveNext()
        {
            return Base.MoveNext();
        }

        /// <summary>
        ///     Sets the enumerator to the first element in the collection
        /// </summary>
        public void Reset()
        {
            Base.Reset();
        }

        /// <summary>
        ///     Sets the enumerator to the first element in the collection
        /// </summary>
        void IEnumerator.Reset()
        {
            Base.Reset();
        }
    }
}
