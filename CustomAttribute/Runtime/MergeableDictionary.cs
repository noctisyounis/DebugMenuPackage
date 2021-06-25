using System.Collections.Generic;
using UnityEngine;

namespace DebugMenu.CustomAttribute.Runtime
{
    public class MergeableDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {

        #region Publics Methods

        /// <summary>
        /// Merge the value and add it in the mergeable Dictionnary
        /// </summary>
        public void Merge(Dictionary<TKey, TValue> other)
        {
            foreach (var item in other)
            {
                if(!this.ContainsKey(item.Key))
                {
                    this.Add(item.Key, item.Value);
                }else
                {
                    Debug.LogError($"the key {item.Key} was already in the dictionary and didn't merged the value {item.Value}.");
                }
            }
        }

        #endregion
    }
}
