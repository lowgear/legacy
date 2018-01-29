﻿using System.Collections.Generic;
using System.Linq;
using log4net;
using Newtonsoft.Json;
using ProviderProcessing.ProcessReports;
using ProviderProcessing.ProviderDatas;
using ProviderProcessing.References;

namespace ProviderProcessing
{
	public class ProviderProcessor
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(ProviderProcessor));
		private readonly ProviderRepository repo;

	    public ProviderProcessor(ProviderRepository repo)
	    {
	        this.repo = repo;;
	    }

		public ProcessReport ProcessProviderData(string message)
		{
			var data = JsonConvert.DeserializeObject<ProviderData>(message);
			var existingData = repo.FindByProviderId(data.ProviderId);
			if (existingData != null && data.Timestamp < existingData.Timestamp)
			{
				log.InfoFormat("Outdated provider data. ProviderId {0} Received timestamp: {1} database timestamp {2}",
					data.ProviderId, data.Timestamp, existingData.Timestamp);
				return new ProcessReport(false, "Outdated data");
			}
			var errors = ValidateNames(data.Products)
				.Concat(data.Products.SelectMany(ValidatePricesAndMeasureUnitCodes))
				.ToArray();
			if (errors.Any())
			{
				return new ProcessReport(false, "Product validation errors",
					errors);
			}

			if (existingData == null)
			{
				repo.Save(data);
			}
			else if (data.ReplaceData)
			{
				log.InfoFormat("Provider {0} products replaced. Deleted: {1} Added: {2}",
					data.ProviderId, existingData.Products.Length, data.Products.Length);
				repo.RemoveById(existingData.Id);
				repo.Save(data);
			}
			else
			{
				var actualProducts = existingData.Products.Where(p => data.Products.All(d => d.Id != p.Id)).ToList();
				var updatedCount = existingData.Products.Length - actualProducts.Count;
				var newCount = data.Products.Length - updatedCount;
				log.InfoFormat("Provider {0} products update. New: {1} Updated: {2}",
					data.ProviderId, newCount, updatedCount);
				existingData.Products = actualProducts.Concat(data.Products).ToArray();
				repo.Update(existingData);
			}
			log.InfoFormat("New data {0}, Existing data {1}", FormatData(data), FormatData(existingData));
			return new ProcessReport(true, "OK");
		}

		private IEnumerable<ProductValidationResult> ValidateNames(ProductData[] data)
		{
			var reference = GetInstance();
			foreach (var product in data)
			{
				if (!reference.FindCodeByName(product.Name).HasValue)
					yield return new ProductValidationResult(product,
						"Unknown product name", ProductValidationSeverity.Error);
			}
		}

	    public virtual ProductsReference GetInstance()
	    {
	        return ProductsReference.GetInstance();
	    }

	    private IEnumerable<ProductValidationResult> ValidatePricesAndMeasureUnitCodes(ProductData product)
		{
			if (product.Price <= 0)
				yield return new ProductValidationResult(product, "Bad price", ProductValidationSeverity.Warning);
			if (!IsValidMeasureUnitCode(product.MeasureUnitCode))
				yield return new ProductValidationResult(product,
					"Bad units of measure", ProductValidationSeverity.Warning);
		}

		private static bool IsValidMeasureUnitCode(string measureUnitCode)
		{
			var reference = MeasureUnitsReference.GetInstance();
			return reference.FindByCode(measureUnitCode) != null;
		}

		private static string FormatData(ProviderData data)
		{
			return data != null
				? data.Id + " for " + data.ProviderId + " products count " + data.Products.Length
				: "null";
		}
	}
}