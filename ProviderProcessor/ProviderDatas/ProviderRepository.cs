using System;

namespace ProviderProcessing.ProviderDatas
{
	public class ProviderRepository
	{
	    public virtual ProviderData FindByProviderId(Guid providerId)
	    {
	        throw new NotImplementedException();
	    }

	    public virtual void RemoveById(Guid id)
	    {
	        throw new NotImplementedException();
	    }

        public virtual void Save(ProviderData data)
        {
            throw new NotImplementedException();
        }

        public virtual void Update(ProviderData existingData)
        {
            throw new NotImplementedException();
        }
    }
}