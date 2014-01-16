// --------------------------------------------------------------------------
// <copyright file="LookupDataService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Caching;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;

    /// <summary>
    /// LookupData Service.
    /// </summary>
    public class LookupDataService : ILookupDataService
    {
        private const string _CacheKey_AllLookUpData = "CacheKey_AllLookUpData";
        private readonly ILookupDataRepository _lookupDataRepository;
        private ICache _cacheManager;

        public LookupDataService(ILookupDataRepository lookupDataRepository = null)
        {
            this._lookupDataRepository = lookupDataRepository ?? new LookupDataRepository();
            this._cacheManager = CacheService.CreateInstance();
        }

        public IList<LookupData> GetAll(string lookupCode)
        {
            var lookupDataList = this._lookupDataRepository.GetAll(lookupCode);

            return lookupDataList;
        }

        /// <summary>
        /// Get lookup by code and id.
        /// </summary>
        /// <param name="lookupCode">Lookup code.</param>
        /// <param name="lookupId">Lookup id.</param>
        /// <returns>Lookup data.</returns>
        public LookupData Get(string lookupCode, int lookupId)
        {
            Check.Argument.IsNotNegativeOrZero(lookupId, "lookupId");
            LookupData lookupData = new LookupData();
            var lookupDataList = this.GetByLookupCode(lookupCode);
            if (lookupDataList != null && lookupDataList.Count > 0)
            {
                lookupData = lookupDataList.Single(data => data.Id == lookupId);
            }

            return lookupData;
        }

        public IList<LookupData> GetJobTitle(string jobTitle)
        {
            var lookUpDataList = this._lookupDataRepository.GetJobTitle(jobTitle);

            return lookUpDataList;
        }

        public bool Delete(int id)
        {
            bool check = this._lookupDataRepository.Delete(id);
            if (check)
            {
                this.ResetJobTitleCache();
            }

            return check;
        }

        public bool Insert(string name, string lookupCode)
        {
            var check = this._lookupDataRepository.Insert(name, lookupCode);
            if (check)
            {
                this.ResetJobTitleCache();
            }

            return check;
        }

        public bool Update(int id, string name)
        {
            var check = this._lookupDataRepository.Update(id, name);
            if (check)
            {
                this.ResetJobTitleCache();
            }

            return check;
        }

        /// <summary>
        /// Function to reset JobTitle Cache.
        /// </summary>
        private void ResetJobTitleCache() 
        {
            var hashTable = this._cacheManager.Get<Hashtable>(_CacheKey_AllLookUpData);
            var lookUpCode = LookupCodeName.JobTitle.ToLower();
            if (hashTable == null)
            {
                hashTable = new Hashtable();
            }

            var listJobTile = this.GetAll(LookupCodeName.JobTitle);
            hashTable[lookUpCode] = listJobTile;
            this._cacheManager.Set<Hashtable>(_CacheKey_AllLookUpData, hashTable, TimeSpan.FromHours(1));
        }

        /// <summary>
        /// Get lookup data from hash table which is cached in memory.
        /// </summary>
        /// <param name="lookUpCode">Lookup code.</param>
        /// <returns>List lookup data.</returns>
        private IList<LookupData> GetByLookupCode(string lookUpCode)
        {
            Check.Argument.IsNotEmpty(lookUpCode, "lookUpCode");

            var hashTable = this._cacheManager.Get<Hashtable>(_CacheKey_AllLookUpData);
            lookUpCode = lookUpCode.ToLower();
            if (hashTable == null)
            {
                hashTable = new Hashtable();
            }

            if (!hashTable.ContainsKey(lookUpCode))
            {
                var lookUpDataList = this._lookupDataRepository.GetAll(lookUpCode);

                if (lookUpDataList != null && lookUpDataList.Count > 0)
                {
                    hashTable.Add(lookUpCode.ToLower(), lookUpDataList);
                    this._cacheManager.Set<Hashtable>(_CacheKey_AllLookUpData, hashTable, TimeSpan.FromHours(1));
                }
            }

            return (IList<LookupData>)hashTable[lookUpCode];
        }
    }
}