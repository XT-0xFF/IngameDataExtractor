using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace CustomPatches.ItemListGeneration
{
	public class QualityFactors
	{
		public static readonly Dictionary<QualityCategory, QualityFactors> All = new Dictionary<QualityCategory, QualityFactors>();

		public static QualityFactors Awful;
		public static QualityFactors Poor;
		public static QualityFactors Normal;
		public static QualityFactors Good;
		public static QualityFactors Excellent;
		public static QualityFactors Masterwork;
		public static QualityFactors Legendary;

		static QualityFactors()
		{
			Awful = new QualityFactors(QualityCategory.Awful)
			{
				Beauty = -0.1f,
				Comfort = 0.76f,
				Protection = 0.6f,
				Insulation = 0.8f,
				Melee = 0.8f,
				RangedAccuracy = 0.8f,
				RangedDamage = 1.0f,
				MarketValue = 0.5f,
				DeteriorationRate = 2.0f,
			};
			Poor = new QualityFactors(QualityCategory.Poor)
			{
				Beauty = 0.5f,
				Comfort = 0.88f,
				Protection = 0.8f,
				Insulation = 0.9f,
				Melee = 0.9f,
				RangedAccuracy = 0.9f,
				RangedDamage = 1.0f,
				MarketValue = 0.75f,
				DeteriorationRate = 1.5f,
			};
			Normal = new QualityFactors(QualityCategory.Normal)
			{
				Beauty = 1.0f,
				Comfort = 1.0f,
				Protection = 1.0f,
				Insulation = 1.0f,
				Melee = 1.0f,
				RangedAccuracy = 1.0f,
				RangedDamage = 1.0f,
				MarketValue = 1.0f,
				DeteriorationRate = 1.0f,
			};
			Good = new QualityFactors(QualityCategory.Good)
			{
				Beauty = 2.0f,
				Comfort = 1.12f,
				Protection = 1.15f,
				Insulation = 1.1f,
				Melee = 1.1f,
				RangedAccuracy = 1.1f,
				RangedDamage = 1.0f,
				MarketValue = 1.25f,
				MaxMarketValueIncrease = 500,
				DeteriorationRate = 0.8f,
			};
			Excellent =	new QualityFactors(QualityCategory.Excellent)
			{
				Beauty = 3.0f,
				Comfort = 1.24f,
				Protection = 1.3f,
				Insulation = 1.2f,
				Melee = 1.2f,
				RangedAccuracy = 1.2f,
				RangedDamage = 1.0f,
				MarketValue = 1.5f,
				MaxMarketValueIncrease = 1000,
				DeteriorationRate = 0.6f,
			};
			Masterwork = new QualityFactors(QualityCategory.Masterwork)
			{
				Beauty = 5.0f,
				Comfort = 1.45f,
				Protection = 1.45f,
				Insulation = 1.5f,
				Melee = 1.45f,
				RangedAccuracy = 1.35f,
				RangedDamage = 1.25f,
				MarketValue = 2.5f,
				MaxMarketValueIncrease = 2000,
				DeteriorationRate = 0.3f,
			};
			Legendary = new QualityFactors(QualityCategory.Legendary)
			{
				Beauty = 8.0f,
				Comfort = 1.7f,
				Protection = 1.8f,
				Insulation = 1.8f,
				Melee = 1.65f,
				RangedAccuracy = 1.5f,
				RangedDamage = 1.5f,
				MarketValue = 5.0f,
				MaxMarketValueIncrease = 3000,
				DeteriorationRate = 0.1f,
			};
		}

		private QualityFactors(QualityCategory quality)
		{
			Quality = quality;
			All.Add(quality, this);
		}

		public QualityCategory Quality;

		public float Beauty;
		public float CalcBeauty(float value) => value * Beauty;

		public float Comfort;
		public float CalcComfort(float value) => value * Comfort;

		public float Protection;
		public float CalcProtection(float value) => value * Protection;

		public float Insulation;
		public float CalcInsulation(float value) => value * Insulation;

		public float Melee;
		public float CalcMelee(float value) => value * Melee;

		public float RangedAccuracy;
		public float CalcRangedAccuracy(float value) => Math.Min(value * RangedAccuracy, 1.0f);

		public float RangedDamage;
		public float CalcRangedDamage(float value) => value * RangedDamage;

		public float MarketValue;
		public float MaxMarketValueIncrease = float.MaxValue;
		public float CalcMarketValue(float value) => Math.Min(value * MarketValue, value + MaxMarketValueIncrease);

		public float DeteriorationRate;
		public float CalcDeteriorationRate(float value) => value * DeteriorationRate;

		public override string ToString() => (int)Quality + " " + Quality.ToString();
	}
}
