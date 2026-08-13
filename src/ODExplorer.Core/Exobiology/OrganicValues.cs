using System;
using System.Collections.Generic;

namespace ODUtils.Exobiology;

public static class OrganicValues
{
	public static readonly DateTime NewPriceDate = new DateTime(2022, 11, 29, 7, 0, 0, DateTimeKind.Utc);

	private static readonly Dictionary<string, OrganicInfo> oldBioValues = new Dictionary<string, OrganicInfo>(StringComparer.InvariantCultureIgnoreCase)
	{
		{
			"$Codex_Ent_Aleoids_01_Name;",
			new OrganicInfo("Aleoida Arcus", 379300L, 150)
		},
		{
			"$Codex_Ent_Aleoids_02_Name;",
			new OrganicInfo("Aleoida Coronamus", 339100L, 150)
		},
		{
			"$Codex_Ent_Aleoids_03_Name;",
			new OrganicInfo("Aleoida Spica", 208900L, 150)
		},
		{
			"$Codex_Ent_Aleoids_04_Name;",
			new OrganicInfo("Aleoida Laminiae", 208900L, 150)
		},
		{
			"$Codex_Ent_Aleoids_05_Name;",
			new OrganicInfo("Aleoida Gravis", 596500L, 150)
		},
		{
			"$Codex_Ent_Bacterial_01_Name;",
			new OrganicInfo("Bacterium Aurasus", 78500L, 500)
		},
		{
			"$Codex_Ent_Bacterial_02_Name;",
			new OrganicInfo("Bacterium Nebulus", 296300L, 500)
		},
		{
			"$Codex_Ent_Bacterial_03_Name;",
			new OrganicInfo("Bacterium Scopulum", 280600L, 500)
		},
		{
			"$Codex_Ent_Bacterial_04_Name;",
			new OrganicInfo("Bacterium Acies", 50000L, 500)
		},
		{
			"$Codex_Ent_Bacterial_05_Name;",
			new OrganicInfo("Bacterium Vesicula", 56100L, 500)
		},
		{
			"$Codex_Ent_Bacterial_06_Name;",
			new OrganicInfo("Bacterium Alcyoneum", 1658500L, 500)
		},
		{
			"$Codex_Ent_Bacterial_07_Name;",
			new OrganicInfo("Bacterium Tela", 135600L, 500)
		},
		{
			"$Codex_Ent_Bacterial_08_Name;",
			new OrganicInfo("Bacterium Informem", 426200L, 500)
		},
		{
			"$Codex_Ent_Bacterial_09_Name;",
			new OrganicInfo("Bacterium Volu", 400500L, 500)
		},
		{
			"$Codex_Ent_Bacterial_10_Name;",
			new OrganicInfo("Bacterium Bullaris", 89900L, 500)
		},
		{
			"$Codex_Ent_Bacterial_11_Name;",
			new OrganicInfo("Bacterium Omentum", 267400L, 500)
		},
		{
			"$Codex_Ent_Bacterial_12_Name;",
			new OrganicInfo("Bacterium Cerbrus", 121300L, 500)
		},
		{
			"$Codex_Ent_Bacterial_13_Name;",
			new OrganicInfo("Bacterium Verrata", 233300L, 500)
		},
		{
			"$Codex_Ent_Cactoid_01_Name;",
			new OrganicInfo("Cactoida Cortexum", 222500L, 300)
		},
		{
			"$Codex_Ent_Cactoid_02_Name;",
			new OrganicInfo("Cactoida Lapis", 164000L, 300)
		},
		{
			"$Codex_Ent_Cactoid_03_Name;",
			new OrganicInfo("Cactoida Vermis", 711500L, 300)
		},
		{
			"$Codex_Ent_Cactoid_04_Name;",
			new OrganicInfo("Cactoida Pullulanata", 222500L, 300)
		},
		{
			"$Codex_Ent_Cactoid_05_Name;",
			new OrganicInfo("Cactoida Peperatis", 184000L, 300)
		},
		{
			"$Codex_Ent_Clypeus_01_Name;",
			new OrganicInfo("Clypeus Lacrimam", 426200L, 150)
		},
		{
			"$Codex_Ent_Clypeus_02_Name;",
			new OrganicInfo("Clypeus Margaritus", 557800L, 150)
		},
		{
			"$Codex_Ent_Clypeus_03_Name;",
			new OrganicInfo("Clypeus Speculumi", 711500L, 150)
		},
		{
			"$Codex_Ent_Conchas_01_Name;",
			new OrganicInfo("Concha Renibus", 264300L, 150)
		},
		{
			"$Codex_Ent_Conchas_02_Name;",
			new OrganicInfo("Concha Aureolas", 400500L, 150)
		},
		{
			"$Codex_Ent_Conchas_03_Name;",
			new OrganicInfo("Concha Labiata", 157100L, 150)
		},
		{
			"$Codex_Ent_Conchas_04_Name;",
			new OrganicInfo("Concha Biconcavis", 806300L, 150)
		},
		{
			"$Codex_Ent_Cone_Name;",
			new OrganicInfo("Bark Mounds", 1000000L, 100)
		},
		{
			"$Codex_Ent_Electricae_01_Name;",
			new OrganicInfo("Electricae Pluma", 339100L, 1000)
		},
		{
			"$Codex_Ent_Electricae_02_Name;",
			new OrganicInfo("Electricae Radialem", 339100L, 1000)
		},
		{
			"$Codex_Ent_Fonticulus_01_Name;",
			new OrganicInfo("Fonticulua Segmentatus", 806300L, 500)
		},
		{
			"$Codex_Ent_Fonticulus_02_Name;",
			new OrganicInfo("Fonticulua Campestris", 63600L, 500)
		},
		{
			"$Codex_Ent_Fonticulus_03_Name;",
			new OrganicInfo("Fonticulua Upupam", 315300L, 500)
		},
		{
			"$Codex_Ent_Fonticulus_04_Name;",
			new OrganicInfo("Fonticulua Lapida", 195600L, 500)
		},
		{
			"$Codex_Ent_Fonticulus_05_Name;",
			new OrganicInfo("Fonticulua Fluctus", 900000L, 500)
		},
		{
			"$Codex_Ent_Fonticulus_06_Name;",
			new OrganicInfo("Fonticulua Digitos", 127700L, 500)
		},
		{
			"$Codex_Ent_Fumerolas_01_Name;",
			new OrganicInfo("Fumerola Carbosis", 339100L, 100)
		},
		{
			"$Codex_Ent_Fumerolas_02_Name;",
			new OrganicInfo("Fumerola Extremus", 711500L, 100)
		},
		{
			"$Codex_Ent_Fumerolas_03_Name;",
			new OrganicInfo("Fumerola Nitris", 389400L, 100)
		},
		{
			"$Codex_Ent_Fumerolas_04_Name;",
			new OrganicInfo("Fumerola Aquatis", 339100L, 100)
		},
		{
			"$Codex_Ent_Fungoids_01_Name;",
			new OrganicInfo("Fungoida Setisis", 120200L, 300)
		},
		{
			"$Codex_Ent_Fungoids_02_Name;",
			new OrganicInfo("Fungoida Stabitis", 174000L, 300)
		},
		{
			"$Codex_Ent_Fungoids_03_Name;",
			new OrganicInfo("Fungoida Bullarum", 224100L, 300)
		},
		{
			"$Codex_Ent_Fungoids_04_Name;",
			new OrganicInfo("Fungoida Gelata", 206300L, 300)
		},
		{
			"$Codex_Ent_Osseus_01_Name;",
			new OrganicInfo("Osseus Fractus", 239400L, 800)
		},
		{
			"$Codex_Ent_Osseus_02_Name;",
			new OrganicInfo("Osseus Discus", 596500L, 800)
		},
		{
			"$Codex_Ent_Osseus_03_Name;",
			new OrganicInfo("Osseus Spiralis", 159900L, 800)
		},
		{
			"$Codex_Ent_Osseus_04_Name;",
			new OrganicInfo("Osseus Pumice", 197800L, 800)
		},
		{
			"$Codex_Ent_Osseus_05_Name;",
			new OrganicInfo("Osseus Cornibus", 109500L, 800)
		},
		{
			"$Codex_Ent_Osseus_06_Name;",
			new OrganicInfo("Osseus Pellebantus", 477700L, 800)
		},
		{
			"$Codex_Ent_Recepta_01_Name;",
			new OrganicInfo("Recepta Umbrux", 596500L, 150)
		},
		{
			"$Codex_Ent_Recepta_02_Name;",
			new OrganicInfo("Recepta Deltahedronix", 711500L, 150)
		},
		{
			"$Codex_Ent_Recepta_03_Name;",
			new OrganicInfo("Recepta Conditivus", 645700L, 150)
		},
		{
			"$Codex_Ent_Seed_Name;",
			new OrganicInfo("Roseum Brain Tree", 1000000L, 100)
		},
		{
			"$Codex_Ent_Shrubs_01_Name;",
			new OrganicInfo("Frutexa Flabellum", 127900L, 150)
		},
		{
			"$Codex_Ent_Shrubs_02_Name;",
			new OrganicInfo("Frutexa Acus", 400500L, 150)
		},
		{
			"$Codex_Ent_Shrubs_03_Name;",
			new OrganicInfo("Frutexa Metallicum", 118100L, 150)
		},
		{
			"$Codex_Ent_Shrubs_04_Name;",
			new OrganicInfo("Frutexa Flammasis", 500100L, 150)
		},
		{
			"$Codex_Ent_Shrubs_05_Name;",
			new OrganicInfo("Frutexa Fera", 118100L, 150)
		},
		{
			"$Codex_Ent_Shrubs_06_Name;",
			new OrganicInfo("Frutexa Sponsae", 326500L, 150)
		},
		{
			"$Codex_Ent_Shrubs_07_Name;",
			new OrganicInfo("Frutexa Collum", 118500L, 150)
		},
		{
			"$Codex_Ent_SphereEFGH_01_Name;",
			new OrganicInfo("Rubeum Bioluminescent Anemone", 1000000L, 100)
		},
		{
			"$Codex_Ent_SphereEFGH_02_Name;",
			new OrganicInfo("Prasinum Bioluminescent Anemone", 1000000L, 100)
		},
		{
			"$Codex_Ent_SphereEFGH_03_Name;",
			new OrganicInfo("Roseum Bioluminescent Anemone", 1000000L, 100)
		},
		{
			"$Codex_Ent_SphereEFGH_Name;",
			new OrganicInfo("Blatteum Bioluminescent Anemone", 1000000L, 100)
		},
		{
			"$Codex_Ent_Stratum_01_Name;",
			new OrganicInfo("Stratum Excutitus", 162200L, 500)
		},
		{
			"$Codex_Ent_Stratum_02_Name;",
			new OrganicInfo("Stratum Paleas", 102500L, 500)
		},
		{
			"$Codex_Ent_Stratum_03_Name;",
			new OrganicInfo("Stratum Laminamus", 179500L, 500)
		},
		{
			"$Codex_Ent_Stratum_04_Name;",
			new OrganicInfo("Stratum Araneamus", 162200L, 500)
		},
		{
			"$Codex_Ent_Stratum_05_Name;",
			new OrganicInfo("Stratum Limaxus", 102500L, 500)
		},
		{
			"$Codex_Ent_Stratum_06_Name;",
			new OrganicInfo("Stratum Cucumisis", 711500L, 500)
		},
		{
			"$Codex_Ent_Stratum_07_Name;",
			new OrganicInfo("Stratum Tectonicas", 806300L, 500)
		},
		{
			"$Codex_Ent_Stratum_08_Name;",
			new OrganicInfo("Stratum Fringus", 162200L, 500)
		},
		{
			"$Codex_Ent_TubeABCD_03_Name;",
			new OrganicInfo("Caeruleum Sinuous Tubers", 1000000L, 100)
		},
		{
			"$Codex_Ent_TubeEFGH_Name;",
			new OrganicInfo("Blatteum Sinuous Tubers", 1000000L, 200)
		},
		{
			"$Codex_Ent_Tubus_01_Name;",
			new OrganicInfo("Tubus Conifer", 315300L, 800)
		},
		{
			"$Codex_Ent_Tubus_02_Name;",
			new OrganicInfo("Tubus Sororibus", 557800L, 800)
		},
		{
			"$Codex_Ent_Tubus_03_Name;",
			new OrganicInfo("Tubus Cavas", 171900L, 800)
		},
		{
			"$Codex_Ent_Tubus_04_Name;",
			new OrganicInfo("Tubus Rosarium", 400500L, 800)
		},
		{
			"$Codex_Ent_Tubus_05_Name;",
			new OrganicInfo("Tubus Compagibus", 102700L, 800)
		},
		{
			"$Codex_Ent_Tussocks_01_Name;",
			new OrganicInfo("Tussock Pennata", 320700L, 200)
		},
		{
			"$Codex_Ent_Tussocks_02_Name;",
			new OrganicInfo("Tussock Ventusa", 201300L, 200)
		},
		{
			"$Codex_Ent_Tussocks_03_Name;",
			new OrganicInfo("Tussock Ignis", 130100L, 200)
		},
		{
			"$Codex_Ent_Tussocks_04_Name;",
			new OrganicInfo("Tussock Cultro", 125600L, 200)
		},
		{
			"$Codex_Ent_Tussocks_05_Name;",
			new OrganicInfo("Tussock Catena", 125600L, 200)
		},
		{
			"$Codex_Ent_Tussocks_06_Name;",
			new OrganicInfo("Tussock Pennatis", 59600L, 200)
		},
		{
			"$Codex_Ent_Tussocks_07_Name;",
			new OrganicInfo("Tussock Serrati", 258700L, 200)
		},
		{
			"$Codex_Ent_Tussocks_08_Name;",
			new OrganicInfo("Tussock Albata", 202500L, 200)
		},
		{
			"$Codex_Ent_Tussocks_09_Name;",
			new OrganicInfo("Tussock Propagito", 71300L, 200)
		},
		{
			"$Codex_Ent_Tussocks_10_Name;",
			new OrganicInfo("Tussock Divisa", 125600L, 200)
		},
		{
			"$Codex_Ent_Tussocks_11_Name;",
			new OrganicInfo("Tussock Caputus", 213100L, 200)
		},
		{
			"$Codex_Ent_Tussocks_12_Name;",
			new OrganicInfo("Tussock Triticum", 400500L, 200)
		},
		{
			"$Codex_Ent_Tussocks_13_Name;",
			new OrganicInfo("Tussock Stigmasis", 806300L, 200)
		},
		{
			"$Codex_Ent_Tussocks_14_Name;",
			new OrganicInfo("Tussock Virgam", 645700L, 200)
		},
		{
			"$Codex_Ent_Tussocks_15_Name;",
			new OrganicInfo("Tussock Capillum", 370000L, 200)
		},
		{
			"$Codex_Ent_Vents_Name;",
			new OrganicInfo("Amphora Plant", 1000000L, 100)
		}
	};

	private static readonly Dictionary<string, OrganicInfo> bioValues = new Dictionary<string, OrganicInfo>(StringComparer.InvariantCultureIgnoreCase)
	{
		{
			"$Codex_Ent_Aleoids_01_Name;",
			new OrganicInfo("Aleoida Arcus", 7252500L, 150)
		},
		{
			"$Codex_Ent_Aleoids_02_Name;",
			new OrganicInfo("Aleoida Coronamus", 6284600L, 150)
		},
		{
			"$Codex_Ent_Aleoids_03_Name;",
			new OrganicInfo("Aleoida Spica", 3385200L, 150)
		},
		{
			"$Codex_Ent_Aleoids_04_Name;",
			new OrganicInfo("Aleoida Laminiae", 3385200L, 150)
		},
		{
			"$Codex_Ent_Aleoids_05_Name;",
			new OrganicInfo("Aleoida Gravis", 12934900L, 150)
		},
		{
			"$Codex_Ent_Bacterial_01_Name;",
			new OrganicInfo("Bacterium Aurasus", 1000000L, 500)
		},
		{
			"$Codex_Ent_Bacterial_02_Name;",
			new OrganicInfo("Bacterium Nebulus", 5289900L, 500)
		},
		{
			"$Codex_Ent_Bacterial_03_Name;",
			new OrganicInfo("Bacterium Scopulum", 4934500L, 500)
		},
		{
			"$Codex_Ent_Bacterial_04_Name;",
			new OrganicInfo("Bacterium Acies", 1000000L, 500)
		},
		{
			"$Codex_Ent_Bacterial_05_Name;",
			new OrganicInfo("Bacterium Vesicula", 1000000L, 500)
		},
		{
			"$Codex_Ent_Bacterial_06_Name;",
			new OrganicInfo("Bacterium Alcyoneum", 1658500L, 500)
		},
		{
			"$Codex_Ent_Bacterial_07_Name;",
			new OrganicInfo("Bacterium Tela", 1949000L, 500)
		},
		{
			"$Codex_Ent_Bacterial_08_Name;",
			new OrganicInfo("Bacterium Informem", 8418000L, 500)
		},
		{
			"$Codex_Ent_Bacterial_09_Name;",
			new OrganicInfo("Bacterium Volu", 7774700L, 500)
		},
		{
			"$Codex_Ent_Bacterial_10_Name;",
			new OrganicInfo("Bacterium Bullaris", 1152500L, 500)
		},
		{
			"$Codex_Ent_Bacterial_11_Name;",
			new OrganicInfo("Bacterium Omentum", 4638900L, 500)
		},
		{
			"$Codex_Ent_Bacterial_12_Name;",
			new OrganicInfo("Bacterium Cerbrus", 1689800L, 500)
		},
		{
			"$Codex_Ent_Bacterial_13_Name;",
			new OrganicInfo("Bacterium Verrata", 3897000L, 500)
		},
		{
			"$Codex_Ent_Cactoid_01_Name;",
			new OrganicInfo("Cactoida Cortexum", 3667600L, 300)
		},
		{
			"$Codex_Ent_Cactoid_02_Name;",
			new OrganicInfo("Cactoida Lapis", 2483600L, 300)
		},
		{
			"$Codex_Ent_Cactoid_03_Name;",
			new OrganicInfo("Cactoida Vermis", 16202800L, 300)
		},
		{
			"$Codex_Ent_Cactoid_04_Name;",
			new OrganicInfo("Cactoida Pullulanata", 3667600L, 300)
		},
		{
			"$Codex_Ent_Cactoid_05_Name;",
			new OrganicInfo("Cactoida Peperatis", 2483600L, 300)
		},
		{
			"$Codex_Ent_Clypeus_01_Name;",
			new OrganicInfo("Clypeus Lacrimam", 8418000L, 150)
		},
		{
			"$Codex_Ent_Clypeus_02_Name;",
			new OrganicInfo("Clypeus Margaritus", 11873200L, 150)
		},
		{
			"$Codex_Ent_Clypeus_03_Name;",
			new OrganicInfo("Clypeus Speculumi", 16202800L, 150)
		},
		{
			"$Codex_Ent_Conchas_01_Name;",
			new OrganicInfo("Concha Renibus", 4572400L, 150)
		},
		{
			"$Codex_Ent_Conchas_02_Name;",
			new OrganicInfo("Concha Aureolas", 7774700L, 150)
		},
		{
			"$Codex_Ent_Conchas_03_Name;",
			new OrganicInfo("Concha Labiata", 2352400L, 150)
		},
		{
			"$Codex_Ent_Conchas_04_Name;",
			new OrganicInfo("Concha Biconcavis", 19010800L, 150)
		},
		{
			"$Codex_Ent_Cone_Name;",
			new OrganicInfo("Bark Mounds", 1471900L, 100)
		},
		{
			"$Codex_Ent_Electricae_01_Name;",
			new OrganicInfo("Electricae Pluma", 6284600L, 1000)
		},
		{
			"$Codex_Ent_Electricae_02_Name;",
			new OrganicInfo("Electricae Radialem", 6284600L, 1000)
		},
		{
			"$Codex_Ent_Fonticulus_01_Name;",
			new OrganicInfo("Fonticulua Segmentatus", 19010800L, 500)
		},
		{
			"$Codex_Ent_Fonticulus_02_Name;",
			new OrganicInfo("Fonticulua Campestris", 1000000L, 500)
		},
		{
			"$Codex_Ent_Fonticulus_03_Name;",
			new OrganicInfo("Fonticulua Upupam", 5727600L, 500)
		},
		{
			"$Codex_Ent_Fonticulus_04_Name;",
			new OrganicInfo("Fonticulua Lapida", 3111000L, 500)
		},
		{
			"$Codex_Ent_Fonticulus_05_Name;",
			new OrganicInfo("Fonticulua Fluctus", 20000000L, 500)
		},
		{
			"$Codex_Ent_Fonticulus_06_Name;",
			new OrganicInfo("Fonticulua Digitos", 1804100L, 500)
		},
		{
			"$Codex_Ent_Fumerolas_01_Name;",
			new OrganicInfo("Fumerola Carbosis", 6284600L, 100)
		},
		{
			"$Codex_Ent_Fumerolas_02_Name;",
			new OrganicInfo("Fumerola Extremus", 16202800L, 100)
		},
		{
			"$Codex_Ent_Fumerolas_03_Name;",
			new OrganicInfo("Fumerola Nitris", 7500900L, 100)
		},
		{
			"$Codex_Ent_Fumerolas_04_Name;",
			new OrganicInfo("Fumerola Aquatis", 6284600L, 100)
		},
		{
			"$Codex_Ent_Fungoids_01_Name;",
			new OrganicInfo("Fungoida Setisis", 1670100L, 300)
		},
		{
			"$Codex_Ent_Fungoids_02_Name;",
			new OrganicInfo("Fungoida Stabitis", 2680300L, 300)
		},
		{
			"$Codex_Ent_Fungoids_03_Name;",
			new OrganicInfo("Fungoida Bullarum", 3703200L, 300)
		},
		{
			"$Codex_Ent_Fungoids_04_Name;",
			new OrganicInfo("Fungoida Gelata", 3330300L, 300)
		},
		{
			"$Codex_Ent_Osseus_01_Name;",
			new OrganicInfo("Osseus Fractus", 4027800L, 800)
		},
		{
			"$Codex_Ent_Osseus_02_Name;",
			new OrganicInfo("Osseus Discus", 12934900L, 800)
		},
		{
			"$Codex_Ent_Osseus_03_Name;",
			new OrganicInfo("Osseus Spiralis", 2404700L, 800)
		},
		{
			"$Codex_Ent_Osseus_04_Name;",
			new OrganicInfo("Osseus Pumice", 3156300L, 800)
		},
		{
			"$Codex_Ent_Osseus_05_Name;",
			new OrganicInfo("Osseus Cornibus", 1483000L, 800)
		},
		{
			"$Codex_Ent_Osseus_06_Name;",
			new OrganicInfo("Osseus Pellebantus", 9739000L, 800)
		},
		{
			"$Codex_Ent_Recepta_01_Name;",
			new OrganicInfo("Recepta Umbrux", 12934900L, 150)
		},
		{
			"$Codex_Ent_Recepta_02_Name;",
			new OrganicInfo("Recepta Deltahedronix", 16202800L, 150)
		},
		{
			"$Codex_Ent_Recepta_03_Name;",
			new OrganicInfo("Recepta Conditivus", 14313700L, 150)
		},
		{
			"$Codex_Ent_Seed_Name;",
			new OrganicInfo("Roseum Brain Tree", 1593700L, 100)
		},
		{
			"$Codex_Ent_Shrubs_01_Name;",
			new OrganicInfo("Frutexa Flabellum", 1808900L, 150)
		},
		{
			"$Codex_Ent_Shrubs_02_Name;",
			new OrganicInfo("Frutexa Acus", 7774700L, 150)
		},
		{
			"$Codex_Ent_Shrubs_03_Name;",
			new OrganicInfo("Frutexa Metallicum", 1632500L, 150)
		},
		{
			"$Codex_Ent_Shrubs_04_Name;",
			new OrganicInfo("Frutexa Flammasis", 10326000L, 150)
		},
		{
			"$Codex_Ent_Shrubs_05_Name;",
			new OrganicInfo("Frutexa Fera", 1632500L, 150)
		},
		{
			"$Codex_Ent_Shrubs_06_Name;",
			new OrganicInfo("Frutexa Sponsae", 5988000L, 150)
		},
		{
			"$Codex_Ent_Shrubs_07_Name;",
			new OrganicInfo("Frutexa Collum", 1639800L, 150)
		},
		{
			"$Codex_Ent_SphereEFGH_01_Name;",
			new OrganicInfo("Rubeum Bioluminescent Anemone", 1499900L, 100)
		},
		{
			"$Codex_Ent_SphereEFGH_02_Name;",
			new OrganicInfo("Prasinum Bioluminescent Anemone", 1499900L, 100)
		},
		{
			"$Codex_Ent_SphereEFGH_03_Name;",
			new OrganicInfo("Roseum Bioluminescent Anemone", 1499900L, 100)
		},
		{
			"$Codex_Ent_SphereEFGH_Name;",
			new OrganicInfo("Blatteum Bioluminescent Anemone", 1499900L, 100)
		},
		{
			"$Codex_Ent_Stratum_01_Name;",
			new OrganicInfo("Stratum Excutitus", 2448900L, 500)
		},
		{
			"$Codex_Ent_Stratum_02_Name;",
			new OrganicInfo("Stratum Paleas", 1362000L, 500)
		},
		{
			"$Codex_Ent_Stratum_03_Name;",
			new OrganicInfo("Stratum Laminamus", 2788300L, 500)
		},
		{
			"$Codex_Ent_Stratum_04_Name;",
			new OrganicInfo("Stratum Araneamus", 2448900L, 500)
		},
		{
			"$Codex_Ent_Stratum_05_Name;",
			new OrganicInfo("Stratum Limaxus", 1362000L, 500)
		},
		{
			"$Codex_Ent_Stratum_06_Name;",
			new OrganicInfo("Stratum Cucumisis", 16202800L, 500)
		},
		{
			"$Codex_Ent_Stratum_07_Name;",
			new OrganicInfo("Stratum Tectonicas", 19010800L, 500)
		},
		{
			"$Codex_Ent_Stratum_08_Name;",
			new OrganicInfo("Stratum Fringus", 2638500L, 500)
		},
		{
			"$Codex_Ent_Tube_Name;",
			new OrganicInfo("Roseum Sinuous Tubers", 1514500L, 100)
		},
		{
			"$Codex_Ent_TubeABCD_03_Name;",
			new OrganicInfo("Caeruleum Sinuous Tubers", 1514500L, 100)
		},
		{
			"$Codex_Ent_TubeEFGH_Name;",
			new OrganicInfo("Blatteum Sinuous Tubers", 3425600L, 200)
		},
		{
			"$Codex_Ent_Tubus_01_Name;",
			new OrganicInfo("Tubus Conifer", 2415500L, 800)
		},
		{
			"$Codex_Ent_Tubus_02_Name;",
			new OrganicInfo("Tubus Sororibus", 5727600L, 800)
		},
		{
			"$Codex_Ent_Tubus_03_Name;",
			new OrganicInfo("Tubus Cavas", 11873200L, 800)
		},
		{
			"$Codex_Ent_Tubus_04_Name;",
			new OrganicInfo("Tubus Rosarium", 2637500L, 800)
		},
		{
			"$Codex_Ent_Tubus_05_Name;",
			new OrganicInfo("Tubus Compagibus", 7774700L, 800)
		},
		{
			"$Codex_Ent_Tussocks_01_Name;",
			new OrganicInfo("Tussock Pennata", 5853800L, 200)
		},
		{
			"$Codex_Ent_Tussocks_02_Name;",
			new OrganicInfo("Tussock Ventusa", 3227700L, 200)
		},
		{
			"$Codex_Ent_Tussocks_03_Name;",
			new OrganicInfo("Tussock Ignis", 1849000L, 200)
		},
		{
			"$Codex_Ent_Tussocks_04_Name;",
			new OrganicInfo("Tussock Cultro", 1766600L, 200)
		},
		{
			"$Codex_Ent_Tussocks_05_Name;",
			new OrganicInfo("Tussock Catena", 1766600L, 200)
		},
		{
			"$Codex_Ent_Tussocks_06_Name;",
			new OrganicInfo("Tussock Pennatis", 1000000L, 200)
		},
		{
			"$Codex_Ent_Tussocks_07_Name;",
			new OrganicInfo("Tussock Serrati", 4447100L, 200)
		},
		{
			"$Codex_Ent_Tussocks_08_Name;",
			new OrganicInfo("Tussock Albata", 3252500L, 200)
		},
		{
			"$Codex_Ent_Tussocks_09_Name;",
			new OrganicInfo("Tussock Propagito", 1000000L, 200)
		},
		{
			"$Codex_Ent_Tussocks_10_Name;",
			new OrganicInfo("Tussock Divisa", 1766600L, 200)
		},
		{
			"$Codex_Ent_Tussocks_11_Name;",
			new OrganicInfo("Tussock Caputus", 3472400L, 200)
		},
		{
			"$Codex_Ent_Tussocks_12_Name;",
			new OrganicInfo("Tussock Triticum", 7774700L, 200)
		},
		{
			"$Codex_Ent_Tussocks_13_Name;",
			new OrganicInfo("Tussock Stigmasis", 19010800L, 200)
		},
		{
			"$Codex_Ent_Tussocks_14_Name;",
			new OrganicInfo("Tussock Virgam", 14313700L, 200)
		},
		{
			"$Codex_Ent_Tussocks_15_Name;",
			new OrganicInfo("Tussock Capillum", 7025800L, 200)
		},
		{
			"$Codex_Ent_Vents_Name;",
			new OrganicInfo("Amphora Plant", 1628800L, 100)
		},
		{
			"$Codex_Ent_Ingensradices_Unicus_Name;",
			new OrganicInfo("Radicoida Unica", 119037L, 100)
		}
	};

	public static string GetSpeciesCodex(string species)
	{
		foreach (KeyValuePair<string, OrganicInfo> bioValue in bioValues)
		{
			if (string.Equals(bioValue.Value.EnglishName, species, StringComparison.OrdinalIgnoreCase))
			{
				return bioValue.Key;
			}
		}
		return string.Empty;
	}

	public static OrganicInfo GetOrganicInfo(string speciesCodexValue, string localName, DateTime saleDate)
	{
		if (string.IsNullOrEmpty(speciesCodexValue))
		{
			return new OrganicInfo(localName, 0L, 100);
		}
		Dictionary<string, OrganicInfo> dictionary = ((saleDate < NewPriceDate) ? oldBioValues : bioValues);
		if (dictionary.TryGetValue(speciesCodexValue, out var value))
		{
			return value;
		}
		return new OrganicInfo(localName, 0L, 100);
	}

	public static string GetSpeciesCodexFromVariantCodex(string variantCodexValue)
	{
		foreach (string key in bioValues.Keys)
		{
			string value = key.Replace("_Name;", "");
			if (variantCodexValue.StartsWith(value))
			{
				return key;
			}
		}
		return string.Empty;
	}

	public static string GetEnglishNameFromGenusCodex(string genusCodex)
	{
		if (1 == 0)
		{
		}
		string result = genusCodex switch
		{
			"$Codex_Ent_Aleoids_Genus_Name;" => "Aleoida", 
			"$Codex_Ent_Bacterial_Genus_Name;" => "Bacterium", 
			"$Codex_Ent_Brancae_Name;" => "Brain Trees", 
			"$Codex_Ent_Cactoid_Genus_Name;" => "Cactoida", 
			"$Codex_Ent_Clypeus_Genus_Name;" => "Clypeus", 
			"$Codex_Ent_Conchas_Genus_Name;" => "Concha", 
			"$Codex_Ent_Cone_Name;" => "Bark Mounds", 
			"$Codex_Ent_Electricae_Genus_Name;" => "Electricae", 
			"$Codex_Ent_Fonticulus_Genus_Name;" => "Fonticulua", 
			"$Codex_Ent_Fumerolas_Genus_Name;" => "Fumerola", 
			"$Codex_Ent_Fungoids_Genus_Name;" => "Fungoida", 
			"$Codex_Ent_Ground_Struct_Ice_Name;" => "Crystalline Shards", 
			"$Codex_Ent_Osseus_Genus_Name;" => "Osseus", 
			"$Codex_Ent_Recepta_Genus_Name;" => "Recepta", 
			"$Codex_Ent_Shrubs_Genus_Name;" => "Frutexa", 
			"$Codex_Ent_Sphere_Name;" => "Bioluminescent Anemone", 
			"$Codex_Ent_Stratum_Genus_Name;" => "Stratum", 
			"$Codex_Ent_Tube_Name;" => "Sinuous Tubers", 
			"$Codex_Ent_Tubus_Genus_Name;" => "Tubus", 
			"$Codex_Ent_Tussocks_Genus_Name;" => "Tussock", 
			"$Codex_Ent_Vents_Name;" => "Amphora Plant", 
			"$Codex_Ent_Ingensradices_Genus_Name;" => "Radicoida", 
			_ => "Unknown", 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public static string GetGenusCodexFromEnglishName(string name)
	{
		if (1 == 0)
		{
		}
		string result = name switch
		{
			"Aleoida" => "$Codex_Ent_Aleoids_Genus_Name;", 
			"Bacterium" => "$Codex_Ent_Bacterial_Genus_Name;", 
			"Brain Trees" => "$Codex_Ent_Brancae_Name;", 
			"Cactoida" => "$Codex_Ent_Cactoid_Genus_Name;", 
			"Clypeus" => "$Codex_Ent_Clypeus_Genus_Name;", 
			"Concha" => "$Codex_Ent_Conchas_Genus_Name;", 
			"Bark Mounds" => "$Codex_Ent_Cone_Name;", 
			"Electricae" => "$Codex_Ent_Electricae_Genus_Name;", 
			"Fonticulua" => "$Codex_Ent_Fonticulus_Genus_Name;", 
			"Fumerola" => "$Codex_Ent_Fumerolas_Genus_Name;", 
			"Fungoida" => "$Codex_Ent_Fungoids_Genus_Name;", 
			"Crystalline Shards" => "$Codex_Ent_Ground_Struct_Ice_Name;", 
			"Osseus" => "$Codex_Ent_Osseus_Genus_Name;", 
			"Recepta" => "$Codex_Ent_Recepta_Genus_Name;", 
			"Frutexa" => "$Codex_Ent_Shrubs_Genus_Name;", 
			"Bioluminescent Anemone" => "$Codex_Ent_Sphere_Name;", 
			"Stratum" => "$Codex_Ent_Stratum_Genus_Name;", 
			"Sinuous Tubers" => "$Codex_Ent_Tube_Name;", 
			"Tubus" => "$Codex_Ent_Tubus_Genus_Name;", 
			"Tussock" => "$Codex_Ent_Tussocks_Genus_Name;", 
			"Amphora Plant" => "$Codex_Ent_Vents_Name;", 
			"Radicoida" => "$Codex_Ent_Ingensradices_Genus_Name;", 
			_ => "Unknown", 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public static string GetVariantEnglishName(string variantCodex)
	{
		if (1 == 0)
		{
		}
		string result = variantCodex switch
		{
			"$Codex_Ent_Aleoids_01_Y_Name;" => "Aleoida Arcus - Amethyst", 
			"$Codex_Ent_Aleoids_01_M_Name;" => "Aleoida Arcus - Emerald", 
			"$Codex_Ent_Aleoids_01_A_Name;" => "Aleoida Arcus - Green", 
			"$Codex_Ent_Aleoids_01_W_Name;" => "Aleoida Arcus - Grey", 
			"$Codex_Ent_Aleoids_01_D_Name;" => "Aleoida Arcus - Indigo", 
			"$Codex_Ent_Aleoids_01_L_Name;" => "Aleoida Arcus - Lime", 
			"$Codex_Ent_Aleoids_01_TTS_Name;" => "Aleoida Arcus - Mauve", 
			"$Codex_Ent_Aleoids_01_N_Name;" => "Aleoida Arcus - Ocher", 
			"$Codex_Ent_Aleoids_01_T_Name;" => "Aleoida Arcus - Sage", 
			"$Codex_Ent_Aleoids_01_F_Name;" => "Aleoida Arcus - Teal", 
			"$Codex_Ent_Aleoids_01_K_Name;" => "Aleoida Arcus - Turquoise", 
			"$Codex_Ent_Aleoids_01_B_Name;" => "Aleoida Arcus - Yellow", 
			"$Codex_Ent_Aleoids_02_Y_Name;" => "Aleoida Coronamus - Amethyst", 
			"$Codex_Ent_Aleoids_02_M_Name;" => "Aleoida Coronamus - Emerald", 
			"$Codex_Ent_Aleoids_02_A_Name;" => "Aleoida Coronamus - Green", 
			"$Codex_Ent_Aleoids_02_D_Name;" => "Aleoida Coronamus - Indigo", 
			"$Codex_Ent_Aleoids_02_L_Name;" => "Aleoida Coronamus - Lime", 
			"$Codex_Ent_Aleoids_02_TTS_Name;" => "Aleoida Coronamus - Mauve", 
			"$Codex_Ent_Aleoids_02_N_Name;" => "Aleoida Coronamus - Ocher", 
			"$Codex_Ent_Aleoids_02_T_Name;" => "Aleoida Coronamus - Sage", 
			"$Codex_Ent_Aleoids_02_F_Name;" => "Aleoida Coronamus - Teal", 
			"$Codex_Ent_Aleoids_02_K_Name;" => "Aleoida Coronamus - Turquoise", 
			"$Codex_Ent_Aleoids_02_B_Name;" => "Aleoida Coronamus - Yellow", 
			"$Codex_Ent_Aleoids_05_Y_Name;" => "Aleoida Gravis - Amethyst", 
			"$Codex_Ent_Aleoids_05_M_Name;" => "Aleoida Gravis - Emerald", 
			"$Codex_Ent_Aleoids_05_A_Name;" => "Aleoida Gravis - Green", 
			"$Codex_Ent_Aleoids_05_D_Name;" => "Aleoida Gravis - Indigo", 
			"$Codex_Ent_Aleoids_05_L_Name;" => "Aleoida Gravis - Lime", 
			"$Codex_Ent_Aleoids_05_TTS_Name;" => "Aleoida Gravis - Mauve", 
			"$Codex_Ent_Aleoids_05_N_Name;" => "Aleoida Gravis - Ocher", 
			"$Codex_Ent_Aleoids_05_T_Name;" => "Aleoida Gravis - Sage", 
			"$Codex_Ent_Aleoids_05_F_Name;" => "Aleoida Gravis - Teal", 
			"$Codex_Ent_Aleoids_05_K_Name;" => "Aleoida Gravis - Turquoise", 
			"$Codex_Ent_Aleoids_05_B_Name;" => "Aleoida Gravis - Yellow", 
			"$Codex_Ent_Aleoids_04_Y_Name;" => "Aleoida Laminiae - Amethyst", 
			"$Codex_Ent_Aleoids_04_M_Name;" => "Aleoida Laminiae - Emerald", 
			"$Codex_Ent_Aleoids_04_A_Name;" => "Aleoida Laminiae - Green", 
			"$Codex_Ent_Aleoids_04_W_Name;" => "Aleoida Laminiae - Grey", 
			"$Codex_Ent_Aleoids_04_D_Name;" => "Aleoida Laminiae - Indigo", 
			"$Codex_Ent_Aleoids_04_L_Name;" => "Aleoida Laminiae - Lime", 
			"$Codex_Ent_Aleoids_04_TTS_Name;" => "Aleoida Laminiae - Mauve", 
			"$Codex_Ent_Aleoids_04_N_Name;" => "Aleoida Laminiae - Ocher", 
			"$Codex_Ent_Aleoids_04_T_Name;" => "Aleoida Laminiae - Sage", 
			"$Codex_Ent_Aleoids_04_F_Name;" => "Aleoida Laminiae - Teal", 
			"$Codex_Ent_Aleoids_04_K_Name;" => "Aleoida Laminiae - Turquoise", 
			"$Codex_Ent_Aleoids_04_B_Name;" => "Aleoida Laminiae - Yellow", 
			"$Codex_Ent_Aleoids_03_M_Name;" => "Aleoida Spica - Emerald", 
			"$Codex_Ent_Aleoids_03_A_Name;" => "Aleoida Spica - Green", 
			"$Codex_Ent_Aleoids_03_D_Name;" => "Aleoida Spica - Indigo", 
			"$Codex_Ent_Aleoids_03_L_Name;" => "Aleoida Spica - Lime", 
			"$Codex_Ent_Aleoids_03_TTS_Name;" => "Aleoida Spica - Mauve", 
			"$Codex_Ent_Aleoids_03_N_Name;" => "Aleoida Spica - Ocher", 
			"$Codex_Ent_Aleoids_03_T_Name;" => "Aleoida Spica - Sage", 
			"$Codex_Ent_Aleoids_03_F_Name;" => "Aleoida Spica - Teal", 
			"$Codex_Ent_Aleoids_03_K_Name;" => "Aleoida Spica - Turquoise", 
			"$Codex_Ent_Aleoids_03_B_Name;" => "Aleoida Spica - Yellow", 
			"$Codex_Ent_Vents_Name;" => "Amphora Plant", 
			"$Codex_Ent_Bacterial_04_Yttrium_Name;" => "Bacterium Acies - Aquamarine", 
			"$Codex_Ent_Bacterial_04_Ruthenium_Name;" => "Bacterium Acies - Cobalt", 
			"$Codex_Ent_Bacterial_04_Antimony_Name;" => "Bacterium Acies - Cyan", 
			"$Codex_Ent_Bacterial_04_Technetium_Name;" => "Bacterium Acies - Lime", 
			"$Codex_Ent_Bacterial_04_Polonium_Name;" => "Bacterium Acies - Magenta", 
			"$Codex_Ent_Bacterial_04_Tellurium_Name;" => "Bacterium Acies - White", 
			"$Codex_Ent_Bacterial_06_W_Name;" => "Bacterium Alcyoneum - Amethyst", 
			"$Codex_Ent_Bacterial_06_G_Name;" => "Bacterium Alcyoneum - Emerald", 
			"$Codex_Ent_Bacterial_06_K_Name;" => "Bacterium Alcyoneum - Green", 
			"$Codex_Ent_Bacterial_06_B_Name;" => "Bacterium Alcyoneum - Grey", 
			"$Codex_Ent_Bacterial_06_N_Name;" => "Bacterium Alcyoneum - Indigo", 
			"$Codex_Ent_Bacterial_06_F_Name;" => "Bacterium Alcyoneum - Lime", 
			"$Codex_Ent_Bacterial_06_TTS_Name;" => "Bacterium Alcyoneum - Maroon", 
			"$Codex_Ent_Bacterial_06_Y_Name;" => "Bacterium Alcyoneum - Mauve", 
			"$Codex_Ent_Bacterial_06_D_Name;" => "Bacterium Alcyoneum - Ocher", 
			"$Codex_Ent_Bacterial_06_Ae_Name;" => "Bacterium Alcyoneum - Orange", 
			"$Codex_Ent_Bacterial_06_T_Name;" => "Bacterium Alcyoneum - Red", 
			"$Codex_Ent_Bacterial_06_L_Name;" => "Bacterium Alcyoneum - Sage", 
			"$Codex_Ent_Bacterial_06_M_Name;" => "Bacterium Alcyoneum - Teal", 
			"$Codex_Ent_Bacterial_06_O_Name;" => "Bacterium Alcyoneum - Turquoise", 
			"$Codex_Ent_Bacterial_06_A_Name;" => "Bacterium Alcyoneum - Yellow", 
			"$Codex_Ent_Bacterial_01_W_Name;" => "Bacterium Aurasus - Amethyst", 
			"$Codex_Ent_Bacterial_01_G_Name;" => "Bacterium Aurasus - Emerald", 
			"$Codex_Ent_Bacterial_01_K_Name;" => "Bacterium Aurasus - Green", 
			"$Codex_Ent_Bacterial_01_B_Name;" => "Bacterium Aurasus - Grey", 
			"$Codex_Ent_Bacterial_01_N_Name;" => "Bacterium Aurasus - Indigo", 
			"$Codex_Ent_Bacterial_01_F_Name;" => "Bacterium Aurasus - Lime", 
			"$Codex_Ent_Bacterial_01_TTS_Name;" => "Bacterium Aurasus - Maroon", 
			"$Codex_Ent_Bacterial_01_Y_Name;" => "Bacterium Aurasus - Mauve", 
			"$Codex_Ent_Bacterial_01_D_Name;" => "Bacterium Aurasus - Ocher", 
			"$Codex_Ent_Bacterial_01_Ae_Name;" => "Bacterium Aurasus - Orange", 
			"$Codex_Ent_Bacterial_01_T_Name;" => "Bacterium Aurasus - Red", 
			"$Codex_Ent_Bacterial_01_L_Name;" => "Bacterium Aurasus - Sage", 
			"$Codex_Ent_Bacterial_01_M_Name;" => "Bacterium Aurasus - Teal", 
			"$Codex_Ent_Bacterial_01_O_Name;" => "Bacterium Aurasus - Turquoise", 
			"$Codex_Ent_Bacterial_01_A_Name;" => "Bacterium Aurasus - Yellow", 
			"$Codex_Ent_Bacterial_10_Ruthenium_Name;" => "Bacterium Bullaris - Aquamarine", 
			"$Codex_Ent_Bacterial_10_Antimony_Name;" => "Bacterium Bullaris - Cobalt", 
			"$Codex_Ent_Bacterial_10_Technetium_Name;" => "Bacterium Bullaris - Gold", 
			"$Codex_Ent_Bacterial_10_Tellurium_Name;" => "Bacterium Bullaris - Lime", 
			"$Codex_Ent_Bacterial_10_Yttrium_Name;" => "Bacterium Bullaris - Red", 
			"$Codex_Ent_Bacterial_10_Polonium_Name;" => "Bacterium Bullaris - Yellow", 
			"$Codex_Ent_Bacterial_12_W_Name;" => "Bacterium Cerbrus - Amethyst", 
			"$Codex_Ent_Bacterial_12_G_Name;" => "Bacterium Cerbrus - Emerald", 
			"$Codex_Ent_Bacterial_12_K_Name;" => "Bacterium Cerbrus - Green", 
			"$Codex_Ent_Bacterial_12_B_Name;" => "Bacterium Cerbrus - Grey", 
			"$Codex_Ent_Bacterial_12_N_Name;" => "Bacterium Cerbrus - Indigo", 
			"$Codex_Ent_Bacterial_12_F_Name;" => "Bacterium Cerbrus - Lime", 
			"$Codex_Ent_Bacterial_12_TTS_Name;" => "Bacterium Cerbrus - Maroon", 
			"$Codex_Ent_Bacterial_12_Y_Name;" => "Bacterium Cerbrus - Mauve", 
			"$Codex_Ent_Bacterial_12_D_Name;" => "Bacterium Cerbrus - Ocher", 
			"$Codex_Ent_Bacterial_12_Ae_Name;" => "Bacterium Cerbrus - Orange", 
			"$Codex_Ent_Bacterial_12_T_Name;" => "Bacterium Cerbrus - Red", 
			"$Codex_Ent_Bacterial_12_L_Name;" => "Bacterium Cerbrus - Sage", 
			"$Codex_Ent_Bacterial_12_M_Name;" => "Bacterium Cerbrus - Teal", 
			"$Codex_Ent_Bacterial_12_O_Name;" => "Bacterium Cerbrus - Turquoise", 
			"$Codex_Ent_Bacterial_12_A_Name;" => "Bacterium Cerbrus - Yellow", 
			"$Codex_Ent_Bacterial_08_Technetium_Name;" => "Bacterium Informem - Aquamarine", 
			"$Codex_Ent_Bacterial_08_Yttrium_Name;" => "Bacterium Informem - Cobalt", 
			"$Codex_Ent_Bacterial_08_Ruthenium_Name;" => "Bacterium Informem - Gold", 
			"$Codex_Ent_Bacterial_08_Polonium_Name;" => "Bacterium Informem - Lime", 
			"$Codex_Ent_Bacterial_08_Antimony_Name;" => "Bacterium Informem - Red", 
			"$Codex_Ent_Bacterial_08_Tellurium_Name;" => "Bacterium Informem - Yellow", 
			"$Codex_Ent_Bacterial_02_Yttrium_Name;" => "Bacterium Nebulus - Cobalt", 
			"$Codex_Ent_Bacterial_02_Technetium_Name;" => "Bacterium Nebulus - Cyan", 
			"$Codex_Ent_Bacterial_02_Polonium_Name;" => "Bacterium Nebulus - Gold", 
			"$Codex_Ent_Bacterial_02_Tellurium_Name;" => "Bacterium Nebulus - Green", 
			"$Codex_Ent_Bacterial_02_Antimony_Name;" => "Bacterium Nebulus - Magenta", 
			"$Codex_Ent_Bacterial_02_Ruthenium_Name;" => "Bacterium Nebulus - Orange", 
			"$Codex_Ent_Bacterial_11_Molybdenum_Name;" => "Bacterium Omentum - Aquamarine", 
			"$Codex_Ent_Bacterial_11_Tungsten_Name;" => "Bacterium Omentum - Blue", 
			"$Codex_Ent_Bacterial_11_Cadmium_Name;" => "Bacterium Omentum - Lime", 
			"$Codex_Ent_Bacterial_11_Niobium_Name;" => "Bacterium Omentum - Peach", 
			"$Codex_Ent_Bacterial_11_Tin_Name;" => "Bacterium Omentum - Red", 
			"$Codex_Ent_Bacterial_11_Mercury_Name;" => "Bacterium Omentum - White", 
			"$Codex_Ent_Bacterial_03_Tungsten_Name;" => "Bacterium Scopulum - Aquamarine", 
			"$Codex_Ent_Bacterial_03_Molybdenum_Name;" => "Bacterium Scopulum - Lime", 
			"$Codex_Ent_Bacterial_03_Tin_Name;" => "Bacterium Scopulum - Mulberry", 
			"$Codex_Ent_Bacterial_03_Mercury_Name;" => "Bacterium Scopulum - Peach", 
			"$Codex_Ent_Bacterial_03_Niobium_Name;" => "Bacterium Scopulum - Red", 
			"$Codex_Ent_Bacterial_03_Cadmium_Name;" => "Bacterium Scopulum - White", 
			"$Codex_Ent_Bacterial_07_Tin_Name;" => "Bacterium Tela - Cobalt", 
			"$Codex_Ent_Bacterial_07_Cadmium_Name;" => "Bacterium Tela - Gold", 
			"$Codex_Ent_Bacterial_07_Tungsten_Name;" => "Bacterium Tela - Green", 
			"$Codex_Ent_Bacterial_07_Niobium_Name;" => "Bacterium Tela - Magenta", 
			"$Codex_Ent_Bacterial_07_Mercury_Name;" => "Bacterium Tela - Orange", 
			"$Codex_Ent_Bacterial_07_Molybdenum_Name;" => "Bacterium Tela - Yellow", 
			"$Codex_Ent_Bacterial_13_Tin_Name;" => "Bacterium Verrata - Blue", 
			"$Codex_Ent_Bacterial_13_Tungsten_Name;" => "Bacterium Verrata - Lime", 
			"$Codex_Ent_Bacterial_13_Niobium_Name;" => "Bacterium Verrata - Mulberry", 
			"$Codex_Ent_Bacterial_13_Cadmium_Name;" => "Bacterium Verrata - Peach", 
			"$Codex_Ent_Bacterial_13_Mercury_Name;" => "Bacterium Verrata - Red", 
			"$Codex_Ent_Bacterial_13_Molybdenum_Name;" => "Bacterium Verrata - White", 
			"$Codex_Ent_Bacterial_05_Antimony_Name;" => "Bacterium Vesicula - Cyan", 
			"$Codex_Ent_Bacterial_05_Technetium_Name;" => "Bacterium Vesicula - Gold", 
			"$Codex_Ent_Bacterial_05_Yttrium_Name;" => "Bacterium Vesicula - Lime", 
			"$Codex_Ent_Bacterial_05_Ruthenium_Name;" => "Bacterium Vesicula - Mulberry", 
			"$Codex_Ent_Bacterial_05_Polonium_Name;" => "Bacterium Vesicula - Orange", 
			"$Codex_Ent_Bacterial_05_Tellurium_Name;" => "Bacterium Vesicula - Red", 
			"$Codex_Ent_Bacterial_09_Polonium_Name;" => "Bacterium Volu - Aquamarine", 
			"$Codex_Ent_Bacterial_09_Ruthenium_Name;" => "Bacterium Volu - Cobalt", 
			"$Codex_Ent_Bacterial_09_Tellurium_Name;" => "Bacterium Volu - Cyan", 
			"$Codex_Ent_Bacterial_09_Yttrium_Name;" => "Bacterium Volu - Gold", 
			"$Codex_Ent_Bacterial_09_Technetium_Name;" => "Bacterium Volu - Lime", 
			"$Codex_Ent_Bacterial_09_Antimony_Name;" => "Bacterium Volu - Red", 
			"$Codex_Ent_Cone_Name;" => "Bark Mounds", 
			"$Codex_Ent_SphereEFGH_Name;" => "Blatteum Bioluminescent Anemone", 
			"$Codex_Ent_TubeEFGH_Name;" => "Blatteum Sinuous Tubers", 
			"$Codex_Ent_Cactoid_01_M_Name;" => "Cactoida Cortexum - Amethyst", 
			"$Codex_Ent_Cactoid_01_A_Name;" => "Cactoida Cortexum - Green", 
			"$Codex_Ent_Cactoid_01_L_Name;" => "Cactoida Cortexum - Mauve", 
			"$Codex_Ent_Cactoid_01_Y_Name;" => "Cactoida Cortexum - Ocher", 
			"$Codex_Ent_Cactoid_01_T_Name;" => "Cactoida Cortexum - Orange", 
			"$Codex_Ent_Cactoid_01_TTS_Name;" => "Cactoida Cortexum - Red", 
			"$Codex_Ent_Cactoid_01_N_Name;" => "Cactoida Cortexum - Sage", 
			"$Codex_Ent_Cactoid_01_G_Name;" => "Cactoida Cortexum - Teal", 
			"$Codex_Ent_Cactoid_01_D_Name;" => "Cactoida Cortexum - Turquoise", 
			"$Codex_Ent_Cactoid_01_F_Name;" => "Cactoida Cortexum - Yellow", 
			"$Codex_Ent_Cactoid_02_M_Name;" => "Cactoida Lapis - Amethyst", 
			"$Codex_Ent_Cactoid_02_A_Name;" => "Cactoida Lapis - Green", 
			"$Codex_Ent_Cactoid_02_O_Name;" => "Cactoida Lapis - Grey", 
			"$Codex_Ent_Cactoid_02_W_Name;" => "Cactoida Lapis - Indigo", 
			"$Codex_Ent_Cactoid_02_L_Name;" => "Cactoida Lapis - Mauve", 
			"$Codex_Ent_Cactoid_02_Y_Name;" => "Cactoida Lapis - Ocher", 
			"$Codex_Ent_Cactoid_02_T_Name;" => "Cactoida Lapis - Orange", 
			"$Codex_Ent_Cactoid_02_TTS_Name;" => "Cactoida Lapis - Red", 
			"$Codex_Ent_Cactoid_02_N_Name;" => "Cactoida Lapis - Sage", 
			"$Codex_Ent_Cactoid_02_G_Name;" => "Cactoida Lapis - Teal", 
			"$Codex_Ent_Cactoid_02_D_Name;" => "Cactoida Lapis - Turquoise", 
			"$Codex_Ent_Cactoid_02_F_Name;" => "Cactoida Lapis - Yellow", 
			"$Codex_Ent_Cactoid_05_M_Name;" => "Cactoida Peperatis - Amethyst", 
			"$Codex_Ent_Cactoid_05_A_Name;" => "Cactoida Peperatis - Green", 
			"$Codex_Ent_Cactoid_05_L_Name;" => "Cactoida Peperatis - Mauve", 
			"$Codex_Ent_Cactoid_05_Y_Name;" => "Cactoida Peperatis - Ocher", 
			"$Codex_Ent_Cactoid_05_T_Name;" => "Cactoida Peperatis - Orange", 
			"$Codex_Ent_Cactoid_05_TTS_Name;" => "Cactoida Peperatis - Red", 
			"$Codex_Ent_Cactoid_05_N_Name;" => "Cactoida Peperatis - Sage", 
			"$Codex_Ent_Cactoid_05_G_Name;" => "Cactoida Peperatis - Teal", 
			"$Codex_Ent_Cactoid_05_D_Name;" => "Cactoida Peperatis - Turquoise", 
			"$Codex_Ent_Cactoid_05_F_Name;" => "Cactoida Peperatis - Yellow", 
			"$Codex_Ent_Cactoid_03_M_Name;" => "Cactoida Vermis - Amethyst", 
			"$Codex_Ent_Cactoid_03_A_Name;" => "Cactoida Vermis - Green", 
			"$Codex_Ent_Cactoid_03_O_Name;" => "Cactoida Vermis - Grey", 
			"$Codex_Ent_Cactoid_03_L_Name;" => "Cactoida Vermis - Mauve", 
			"$Codex_Ent_Cactoid_03_Y_Name;" => "Cactoida Vermis - Ocher", 
			"$Codex_Ent_Cactoid_03_T_Name;" => "Cactoida Vermis - Orange", 
			"$Codex_Ent_Cactoid_03_TTS_Name;" => "Cactoida Vermis - Red", 
			"$Codex_Ent_Cactoid_03_N_Name;" => "Cactoida Vermis - Sage", 
			"$Codex_Ent_Cactoid_03_G_Name;" => "Cactoida Vermis - Teal", 
			"$Codex_Ent_Cactoid_03_D_Name;" => "Cactoida Vermis - Turquoise", 
			"$Codex_Ent_Cactoid_03_F_Name;" => "Cactoida Vermis - Yellow", 
			"$Codex_Ent_TubeABCD_03_Name;" => "Caeruleum Sinuous Tubers", 
			"$Codex_Ent_Clypeus_01_G_Name;" => "Clypeus Lacrimam - Amethyst", 
			"$Codex_Ent_Clypeus_01_Y_Name;" => "Clypeus Lacrimam - Green", 
			"$Codex_Ent_Clypeus_01_K_Name;" => "Clypeus Lacrimam - Grey", 
			"$Codex_Ent_Clypeus_01_D_Name;" => "Clypeus Lacrimam - Lime", 
			"$Codex_Ent_Clypeus_01_B_Name;" => "Clypeus Lacrimam - Maroon", 
			"$Codex_Ent_Clypeus_01_F_Name;" => "Clypeus Lacrimam - Mauve", 
			"$Codex_Ent_Clypeus_01_A_Name;" => "Clypeus Lacrimam - Orange", 
			"$Codex_Ent_Clypeus_01_L_Name;" => "Clypeus Lacrimam - Teal", 
			"$Codex_Ent_Clypeus_01_M_Name;" => "Clypeus Lacrimam - Turquoise", 
			"$Codex_Ent_Clypeus_01_N_Name;" => "Clypeus Lacrimam - Yellow", 
			"$Codex_Ent_Clypeus_02_G_Name;" => "Clypeus Margaritus - Amethyst", 
			"$Codex_Ent_Clypeus_02_Y_Name;" => "Clypeus Margaritus - Green", 
			"$Codex_Ent_Clypeus_02_K_Name;" => "Clypeus Margaritus - Grey", 
			"$Codex_Ent_Clypeus_02_D_Name;" => "Clypeus Margaritus - Lime", 
			"$Codex_Ent_Clypeus_02_B_Name;" => "Clypeus Margaritus - Maroon", 
			"$Codex_Ent_Clypeus_02_F_Name;" => "Clypeus Margaritus - Mauve", 
			"$Codex_Ent_Clypeus_02_A_Name;" => "Clypeus Margaritus - Orange", 
			"$Codex_Ent_Clypeus_02_L_Name;" => "Clypeus Margaritus - Teal", 
			"$Codex_Ent_Clypeus_02_M_Name;" => "Clypeus Margaritus - Turquoise", 
			"$Codex_Ent_Clypeus_02_N_Name;" => "Clypeus Margaritus - Yellow", 
			"$Codex_Ent_Clypeus_03_G_Name;" => "Clypeus Speculumi - Amethyst", 
			"$Codex_Ent_Clypeus_03_K_Name;" => "Clypeus Speculumi - Grey", 
			"$Codex_Ent_Clypeus_03_B_Name;" => "Clypeus Speculumi - Maroon", 
			"$Codex_Ent_Clypeus_03_F_Name;" => "Clypeus Speculumi - Mauve", 
			"$Codex_Ent_Clypeus_03_A_Name;" => "Clypeus Speculumi - Orange", 
			"$Codex_Ent_Clypeus_03_M_Name;" => "Clypeus Speculumi - Turquoise", 
			"$Codex_Ent_Clypeus_03_N_Name;" => "Clypeus Speculumi - Yellow", 
			"$Codex_Ent_Conchas_02_N_Name;" => "Concha Aureolas - Emerald", 
			"$Codex_Ent_Conchas_02_D_Name;" => "Concha Aureolas - Green", 
			"$Codex_Ent_Conchas_02_F_Name;" => "Concha Aureolas - Grey", 
			"$Codex_Ent_Conchas_02_B_Name;" => "Concha Aureolas - Indigo", 
			"$Codex_Ent_Conchas_02_L_Name;" => "Concha Aureolas - Orange", 
			"$Codex_Ent_Conchas_02_K_Name;" => "Concha Aureolas - Red", 
			"$Codex_Ent_Conchas_02_A_Name;" => "Concha Aureolas - Teal", 
			"$Codex_Ent_Conchas_02_G_Name;" => "Concha Aureolas - Turquoise", 
			"$Codex_Ent_Conchas_02_Y_Name;" => "Concha Aureolas - Yellow", 
			"$Codex_Ent_Conchas_04_Yttrium_Name;" => "Concha Biconcavis - Gold", 
			"$Codex_Ent_Conchas_04_Ruthenium_Name;" => "Concha Biconcavis - Orange", 
			"$Codex_Ent_Conchas_04_Antimony_Name;" => "Concha Biconcavis - Peach", 
			"$Codex_Ent_Conchas_04_Polonium_Name;" => "Concha Biconcavis - Red", 
			"$Codex_Ent_Conchas_04_Technetium_Name;" => "Concha Biconcavis - White", 
			"$Codex_Ent_Conchas_04_Tellurium_Name;" => "Concha Biconcavis - Yellow", 
			"$Codex_Ent_Conchas_03_N_Name;" => "Concha Labiata - Emerald", 
			"$Codex_Ent_Conchas_03_D_Name;" => "Concha Labiata - Green", 
			"$Codex_Ent_Conchas_03_F_Name;" => "Concha Labiata - Grey", 
			"$Codex_Ent_Conchas_03_B_Name;" => "Concha Labiata - Indigo", 
			"$Codex_Ent_Conchas_03_W_Name;" => "Concha Labiata - Lime", 
			"$Codex_Ent_Conchas_03_L_Name;" => "Concha Labiata - Orange", 
			"$Codex_Ent_Conchas_03_K_Name;" => "Concha Labiata - Red", 
			"$Codex_Ent_Conchas_03_A_Name;" => "Concha Labiata - Teal", 
			"$Codex_Ent_Conchas_03_G_Name;" => "Concha Labiata - Turquoise", 
			"$Codex_Ent_Conchas_03_Y_Name;" => "Concha Labiata - Yellow", 
			"$Codex_Ent_Conchas_01_Tin_Name;" => "Concha Renibus - Aquamarine", 
			"$Codex_Ent_Conchas_01_Niobium_Name;" => "Concha Renibus - Blue", 
			"$Codex_Ent_Conchas_01_Mercury_Name;" => "Concha Renibus - Mulberry", 
			"$Codex_Ent_Conchas_01_Molybdenum_Name;" => "Concha Renibus - Peach", 
			"$Codex_Ent_Conchas_01_Cadmium_Name;" => "Concha Renibus - Red", 
			"$Codex_Ent_Conchas_01_Tungsten_Name;" => "Concha Renibus - White", 
			"$Codex_Ent_Electricae_01_Ruthenium_Name;" => "Electricae Pluma - Blue", 
			"$Codex_Ent_Electricae_01_Antimony_Name;" => "Electricae Pluma - Cobalt", 
			"$Codex_Ent_Electricae_01_Polonium_Name;" => "Electricae Pluma - Cyan", 
			"$Codex_Ent_Electricae_01_Technetium_Name;" => "Electricae Pluma - Magenta", 
			"$Codex_Ent_Electricae_01_Yttrium_Name;" => "Electricae Pluma - Mulberry", 
			"$Codex_Ent_Electricae_01_Tellurium_Name;" => "Electricae Pluma - Red", 
			"$Codex_Ent_Electricae_02_Technetium_Name;" => "Electricae Radialem - Aquamarine", 
			"$Codex_Ent_Electricae_02_Ruthenium_Name;" => "Electricae Radialem - Blue", 
			"$Codex_Ent_Electricae_02_Polonium_Name;" => "Electricae Radialem - Cobalt", 
			"$Codex_Ent_Electricae_02_Antimony_Name;" => "Electricae Radialem - Cyan", 
			"$Codex_Ent_Electricae_02_Yttrium_Name;" => "Electricae Radialem - Green", 
			"$Codex_Ent_Electricae_02_Tellurium_Name;" => "Electricae Radialem - Magenta", 
			"$Codex_Ent_Fonticulus_02_M_Name;" => "Fonticulua Campestris - Amethyst", 
			"$Codex_Ent_Fonticulus_02_K_Name;" => "Fonticulua Campestris - Emerald", 
			"$Codex_Ent_Fonticulus_02_A_Name;" => "Fonticulua Campestris - Green", 
			"$Codex_Ent_Fonticulus_02_O_Name;" => "Fonticulua Campestris - Grey", 
			"$Codex_Ent_Fonticulus_02_B_Name;" => "Fonticulua Campestris - Lime", 
			"$Codex_Ent_Fonticulus_02_Ae_Name;" => "Fonticulua Campestris - Maroon", 
			"$Codex_Ent_Fonticulus_02_L_Name;" => "Fonticulua Campestris - Mauve", 
			"$Codex_Ent_Fonticulus_02_Y_Name;" => "Fonticulua Campestris - Ocher", 
			"$Codex_Ent_Fonticulus_02_T_Name;" => "Fonticulua Campestris - Orange", 
			"$Codex_Ent_Fonticulus_02_TTS_Name;" => "Fonticulua Campestris - Red", 
			"$Codex_Ent_Fonticulus_02_N_Name;" => "Fonticulua Campestris - Sage", 
			"$Codex_Ent_Fonticulus_02_G_Name;" => "Fonticulua Campestris - Teal", 
			"$Codex_Ent_Fonticulus_02_D_Name;" => "Fonticulua Campestris - Turquoise", 
			"$Codex_Ent_Fonticulus_02_F_Name;" => "Fonticulua Campestris - Yellow", 
			"$Codex_Ent_Fonticulus_06_M_Name;" => "Fonticulua Digitos - Amethyst", 
			"$Codex_Ent_Fonticulus_06_K_Name;" => "Fonticulua Digitos - Emerald", 
			"$Codex_Ent_Fonticulus_06_A_Name;" => "Fonticulua Digitos - Green", 
			"$Codex_Ent_Fonticulus_06_B_Name;" => "Fonticulua Digitos - Lime", 
			"$Codex_Ent_Fonticulus_06_L_Name;" => "Fonticulua Digitos - Mauve", 
			"$Codex_Ent_Fonticulus_06_Y_Name;" => "Fonticulua Digitos - Ocher", 
			"$Codex_Ent_Fonticulus_06_T_Name;" => "Fonticulua Digitos - Orange", 
			"$Codex_Ent_Fonticulus_06_TTS_Name;" => "Fonticulua Digitos - Red", 
			"$Codex_Ent_Fonticulus_06_N_Name;" => "Fonticulua Digitos - Sage", 
			"$Codex_Ent_Fonticulus_06_G_Name;" => "Fonticulua Digitos - Teal", 
			"$Codex_Ent_Fonticulus_06_D_Name;" => "Fonticulua Digitos - Turquoise", 
			"$Codex_Ent_Fonticulus_06_F_Name;" => "Fonticulua Digitos - Yellow", 
			"$Codex_Ent_Fonticulus_05_M_Name;" => "Fonticulua Fluctus - Amethyst", 
			"$Codex_Ent_Fonticulus_05_K_Name;" => "Fonticulua Fluctus - Emerald", 
			"$Codex_Ent_Fonticulus_05_A_Name;" => "Fonticulua Fluctus - Green", 
			"$Codex_Ent_Fonticulus_05_B_Name;" => "Fonticulua Fluctus - Lime", 
			"$Codex_Ent_Fonticulus_05_L_Name;" => "Fonticulua Fluctus - Mauve", 
			"$Codex_Ent_Fonticulus_05_T_Name;" => "Fonticulua Fluctus - Orange", 
			"$Codex_Ent_Fonticulus_05_TTS_Name;" => "Fonticulua Fluctus - Red", 
			"$Codex_Ent_Fonticulus_05_N_Name;" => "Fonticulua Fluctus - Sage", 
			"$Codex_Ent_Fonticulus_05_G_Name;" => "Fonticulua Fluctus - Teal", 
			"$Codex_Ent_Fonticulus_05_F_Name;" => "Fonticulua Fluctus - Yellow", 
			"$Codex_Ent_Fonticulus_04_M_Name;" => "Fonticulua Lapida - Amethyst", 
			"$Codex_Ent_Fonticulus_04_K_Name;" => "Fonticulua Lapida - Emerald", 
			"$Codex_Ent_Fonticulus_04_A_Name;" => "Fonticulua Lapida - Green", 
			"$Codex_Ent_Fonticulus_04_O_Name;" => "Fonticulua Lapida - Grey", 
			"$Codex_Ent_Fonticulus_04_B_Name;" => "Fonticulua Lapida - Lime", 
			"$Codex_Ent_Fonticulus_04_Ae_Name;" => "Fonticulua Lapida - Maroon", 
			"$Codex_Ent_Fonticulus_04_L_Name;" => "Fonticulua Lapida - Mauve", 
			"$Codex_Ent_Fonticulus_04_Y_Name;" => "Fonticulua Lapida - Ocher", 
			"$Codex_Ent_Fonticulus_04_T_Name;" => "Fonticulua Lapida - Orange", 
			"$Codex_Ent_Fonticulus_04_TTS_Name;" => "Fonticulua Lapida - Red", 
			"$Codex_Ent_Fonticulus_04_N_Name;" => "Fonticulua Lapida - Sage", 
			"$Codex_Ent_Fonticulus_04_G_Name;" => "Fonticulua Lapida - Teal", 
			"$Codex_Ent_Fonticulus_04_D_Name;" => "Fonticulua Lapida - Turquoise", 
			"$Codex_Ent_Fonticulus_04_F_Name;" => "Fonticulua Lapida - Yellow", 
			"$Codex_Ent_Fonticulus_01_M_Name;" => "Fonticulua Segmentatus - Amethyst", 
			"$Codex_Ent_Fonticulus_01_K_Name;" => "Fonticulua Segmentatus - Emerald", 
			"$Codex_Ent_Fonticulus_01_A_Name;" => "Fonticulua Segmentatus - Green", 
			"$Codex_Ent_Fonticulus_01_B_Name;" => "Fonticulua Segmentatus - Lime", 
			"$Codex_Ent_Fonticulus_01_Ae_Name;" => "Fonticulua Segmentatus - Maroon", 
			"$Codex_Ent_Fonticulus_01_L_Name;" => "Fonticulua Segmentatus - Mauve", 
			"$Codex_Ent_Fonticulus_01_Y_Name;" => "Fonticulua Segmentatus - Ocher", 
			"$Codex_Ent_Fonticulus_01_T_Name;" => "Fonticulua Segmentatus - Orange", 
			"$Codex_Ent_Fonticulus_01_TTS_Name;" => "Fonticulua Segmentatus - Red", 
			"$Codex_Ent_Fonticulus_01_N_Name;" => "Fonticulua Segmentatus - Sage", 
			"$Codex_Ent_Fonticulus_01_G_Name;" => "Fonticulua Segmentatus - Teal", 
			"$Codex_Ent_Fonticulus_01_D_Name;" => "Fonticulua Segmentatus - Turquoise", 
			"$Codex_Ent_Fonticulus_01_F_Name;" => "Fonticulua Segmentatus - Yellow", 
			"$Codex_Ent_Fonticulus_03_M_Name;" => "Fonticulua Upupam - Amethyst", 
			"$Codex_Ent_Fonticulus_03_K_Name;" => "Fonticulua Upupam - Emerald", 
			"$Codex_Ent_Fonticulus_03_A_Name;" => "Fonticulua Upupam - Green", 
			"$Codex_Ent_Fonticulus_03_W_Name;" => "Fonticulua Upupam - Indigo", 
			"$Codex_Ent_Fonticulus_03_B_Name;" => "Fonticulua Upupam - Lime", 
			"$Codex_Ent_Fonticulus_03_Ae_Name;" => "Fonticulua Upupam - Maroon", 
			"$Codex_Ent_Fonticulus_03_L_Name;" => "Fonticulua Upupam - Mauve", 
			"$Codex_Ent_Fonticulus_03_Y_Name;" => "Fonticulua Upupam - Ocher", 
			"$Codex_Ent_Fonticulus_03_T_Name;" => "Fonticulua Upupam - Orange", 
			"$Codex_Ent_Fonticulus_03_TTS_Name;" => "Fonticulua Upupam - Red", 
			"$Codex_Ent_Fonticulus_03_N_Name;" => "Fonticulua Upupam - Sage", 
			"$Codex_Ent_Fonticulus_03_G_Name;" => "Fonticulua Upupam - Teal", 
			"$Codex_Ent_Fonticulus_03_D_Name;" => "Fonticulua Upupam - Turquoise", 
			"$Codex_Ent_Fonticulus_03_F_Name;" => "Fonticulua Upupam - Yellow", 
			"$Codex_Ent_Shrubs_02_G_Name;" => "Frutexa Acus - Emerald", 
			"$Codex_Ent_Shrubs_02_F_Name;" => "Frutexa Acus - Green", 
			"$Codex_Ent_Shrubs_02_M_Name;" => "Frutexa Acus - Grey", 
			"$Codex_Ent_Shrubs_02_D_Name;" => "Frutexa Acus - Indigo", 
			"$Codex_Ent_Shrubs_02_B_Name;" => "Frutexa Acus - Lime", 
			"$Codex_Ent_Shrubs_02_TTS_Name;" => "Frutexa Acus - Mauve", 
			"$Codex_Ent_Shrubs_02_W_Name;" => "Frutexa Acus - Orange", 
			"$Codex_Ent_Shrubs_02_N_Name;" => "Frutexa Acus - Red", 
			"$Codex_Ent_Shrubs_02_L_Name;" => "Frutexa Acus - Teal", 
			"$Codex_Ent_Shrubs_07_G_Name;" => "Frutexa Collum - Emerald", 
			"$Codex_Ent_Shrubs_07_F_Name;" => "Frutexa Collum - Green", 
			"$Codex_Ent_Shrubs_07_M_Name;" => "Frutexa Collum - Grey", 
			"$Codex_Ent_Shrubs_07_D_Name;" => "Frutexa Collum - Indigo", 
			"$Codex_Ent_Shrubs_07_B_Name;" => "Frutexa Collum - Lime", 
			"$Codex_Ent_Shrubs_07_TTS_Name;" => "Frutexa Collum - Mauve", 
			"$Codex_Ent_Shrubs_07_N_Name;" => "Frutexa Collum - Red", 
			"$Codex_Ent_Shrubs_07_L_Name;" => "Frutexa Collum - Teal", 
			"$Codex_Ent_Shrubs_07_O_Name;" => "Frutexa Collum - Yellow", 
			"$Codex_Ent_Shrubs_05_G_Name;" => "Frutexa Fera - Emerald", 
			"$Codex_Ent_Shrubs_05_F_Name;" => "Frutexa Fera - Green", 
			"$Codex_Ent_Shrubs_05_M_Name;" => "Frutexa Fera - Grey", 
			"$Codex_Ent_Shrubs_05_D_Name;" => "Frutexa Fera - Indigo", 
			"$Codex_Ent_Shrubs_05_B_Name;" => "Frutexa Fera - Lime", 
			"$Codex_Ent_Shrubs_05_TTS_Name;" => "Frutexa Fera - Mauve", 
			"$Codex_Ent_Shrubs_05_N_Name;" => "Frutexa Fera - Red", 
			"$Codex_Ent_Shrubs_05_L_Name;" => "Frutexa Fera - Teal", 
			"$Codex_Ent_Shrubs_01_G_Name;" => "Frutexa Flabellum - Emerald", 
			"$Codex_Ent_Shrubs_01_F_Name;" => "Frutexa Flabellum - Green", 
			"$Codex_Ent_Shrubs_01_M_Name;" => "Frutexa Flabellum - Grey", 
			"$Codex_Ent_Shrubs_01_D_Name;" => "Frutexa Flabellum - Indigo", 
			"$Codex_Ent_Shrubs_01_B_Name;" => "Frutexa Flabellum - Lime", 
			"$Codex_Ent_Shrubs_01_TTS_Name;" => "Frutexa Flabellum - Mauve", 
			"$Codex_Ent_Shrubs_01_W_Name;" => "Frutexa Flabellum - Orange", 
			"$Codex_Ent_Shrubs_01_N_Name;" => "Frutexa Flabellum - Red", 
			"$Codex_Ent_Shrubs_01_L_Name;" => "Frutexa Flabellum - Teal", 
			"$Codex_Ent_Shrubs_01_O_Name;" => "Frutexa Flabellum - Yellow", 
			"$Codex_Ent_Shrubs_04_G_Name;" => "Frutexa Flammasis - Emerald", 
			"$Codex_Ent_Shrubs_04_F_Name;" => "Frutexa Flammasis - Green", 
			"$Codex_Ent_Shrubs_04_M_Name;" => "Frutexa Flammasis - Grey", 
			"$Codex_Ent_Shrubs_04_D_Name;" => "Frutexa Flammasis - Indigo", 
			"$Codex_Ent_Shrubs_04_B_Name;" => "Frutexa Flammasis - Lime", 
			"$Codex_Ent_Shrubs_04_TTS_Name;" => "Frutexa Flammasis - Mauve", 
			"$Codex_Ent_Shrubs_04_N_Name;" => "Frutexa Flammasis - Red", 
			"$Codex_Ent_Shrubs_04_L_Name;" => "Frutexa Flammasis - Teal", 
			"$Codex_Ent_Shrubs_03_G_Name;" => "Frutexa Metallicum - Emerald", 
			"$Codex_Ent_Shrubs_03_F_Name;" => "Frutexa Metallicum - Green", 
			"$Codex_Ent_Shrubs_03_M_Name;" => "Frutexa Metallicum - Grey", 
			"$Codex_Ent_Shrubs_03_D_Name;" => "Frutexa Metallicum - Indigo", 
			"$Codex_Ent_Shrubs_03_B_Name;" => "Frutexa Metallicum - Lime", 
			"$Codex_Ent_Shrubs_03_TTS_Name;" => "Frutexa Metallicum - Mauve", 
			"$Codex_Ent_Shrubs_03_N_Name;" => "Frutexa Metallicum - Red", 
			"$Codex_Ent_Shrubs_03_L_Name;" => "Frutexa Metallicum - Teal", 
			"$Codex_Ent_Shrubs_06_G_Name;" => "Frutexa Sponsae - Emerald", 
			"$Codex_Ent_Shrubs_06_F_Name;" => "Frutexa Sponsae - Green", 
			"$Codex_Ent_Shrubs_06_M_Name;" => "Frutexa Sponsae - Grey", 
			"$Codex_Ent_Shrubs_06_D_Name;" => "Frutexa Sponsae - Indigo", 
			"$Codex_Ent_Shrubs_06_B_Name;" => "Frutexa Sponsae - Lime", 
			"$Codex_Ent_Shrubs_06_TTS_Name;" => "Frutexa Sponsae - Mauve", 
			"$Codex_Ent_Shrubs_06_N_Name;" => "Frutexa Sponsae - Red", 
			"$Codex_Ent_Shrubs_06_L_Name;" => "Frutexa Sponsae - Teal", 
			"$Codex_Ent_Fumerolas_04_Tungsten_Name;" => "Fumerola Aquatis - Cobalt", 
			"$Codex_Ent_Fumerolas_04_Molybdenum_Name;" => "Fumerola Aquatis - Cyan", 
			"$Codex_Ent_Fumerolas_04_Niobium_Name;" => "Fumerola Aquatis - Gold", 
			"$Codex_Ent_Fumerolas_04_Cadmium_Name;" => "Fumerola Aquatis - Green", 
			"$Codex_Ent_Fumerolas_04_Tin_Name;" => "Fumerola Aquatis - Orange", 
			"$Codex_Ent_Fumerolas_04_Mercury_Name;" => "Fumerola Aquatis - Yellow", 
			"$Codex_Ent_Fumerolas_01_Niobium_Name;" => "Fumerola Carbosis - Cobalt", 
			"$Codex_Ent_Fumerolas_01_Tin_Name;" => "Fumerola Carbosis - Cyan", 
			"$Codex_Ent_Fumerolas_01_Molybdenum_Name;" => "Fumerola Carbosis - Gold", 
			"$Codex_Ent_Fumerolas_01_Mercury_Name;" => "Fumerola Carbosis - Magenta", 
			"$Codex_Ent_Fumerolas_01_Cadmium_Name;" => "Fumerola Carbosis - Orange", 
			"$Codex_Ent_Fumerolas_01_Tungsten_Name;" => "Fumerola Carbosis - Yellow", 
			"$Codex_Ent_Fumerolas_02_Cadmium_Name;" => "Fumerola Extremus - Aquamarine", 
			"$Codex_Ent_Fumerolas_02_Molybdenum_Name;" => "Fumerola Extremus - Blue", 
			"$Codex_Ent_Fumerolas_02_Mercury_Name;" => "Fumerola Extremus - Lime", 
			"$Codex_Ent_Fumerolas_02_Tungsten_Name;" => "Fumerola Extremus - Mulberry", 
			"$Codex_Ent_Fumerolas_02_Tin_Name;" => "Fumerola Extremus - Peach", 
			"$Codex_Ent_Fumerolas_02_Niobium_Name;" => "Fumerola Extremus - White", 
			"$Codex_Ent_Fumerolas_03_Tungsten_Name;" => "Fumerola Nitris - Aquamarine", 
			"$Codex_Ent_Fumerolas_03_Molybdenum_Name;" => "Fumerola Nitris - Lime", 
			"$Codex_Ent_Fumerolas_03_Tin_Name;" => "Fumerola Nitris - Mulberry", 
			"$Codex_Ent_Fumerolas_03_Mercury_Name;" => "Fumerola Nitris - Peach", 
			"$Codex_Ent_Fumerolas_03_Niobium_Name;" => "Fumerola Nitris - Red", 
			"$Codex_Ent_Fumerolas_03_Cadmium_Name;" => "Fumerola Nitris - White", 
			"$Codex_Ent_Fungoids_03_Tellurium_Name;" => "Fungoida Bullarum - Gold", 
			"$Codex_Ent_Fungoids_03_Ruthenium_Name;" => "Fungoida Bullarum - Magenta", 
			"$Codex_Ent_Fungoids_03_Polonium_Name;" => "Fungoida Bullarum - Mulberry", 
			"$Codex_Ent_Fungoids_03_Yttrium_Name;" => "Fungoida Bullarum - Orange", 
			"$Codex_Ent_Fungoids_03_Technetium_Name;" => "Fungoida Bullarum - Peach", 
			"$Codex_Ent_Fungoids_03_Antimony_Name;" => "Fungoida Bullarum - Red", 
			"$Codex_Ent_Fungoids_04_Cadmium_Name;" => "Fungoida Gelata - Cyan", 
			"$Codex_Ent_Fungoids_04_Niobium_Name;" => "Fungoida Gelata - Green", 
			"$Codex_Ent_Fungoids_04_Mercury_Name;" => "Fungoida Gelata - Lime", 
			"$Codex_Ent_Fungoids_04_Molybdenum_Name;" => "Fungoida Gelata - Mulberry", 
			"$Codex_Ent_Fungoids_04_Tungsten_Name;" => "Fungoida Gelata - Orange", 
			"$Codex_Ent_Fungoids_04_Tin_Name;" => "Fungoida Gelata - Red", 
			"$Codex_Ent_Fungoids_01_Ruthenium_Name;" => "Fungoida Setisis - Gold", 
			"$Codex_Ent_Fungoids_01_Technetium_Name;" => "Fungoida Setisis - Lime", 
			"$Codex_Ent_Fungoids_01_Yttrium_Name;" => "Fungoida Setisis - Orange", 
			"$Codex_Ent_Fungoids_01_Antimony_Name;" => "Fungoida Setisis - Peach", 
			"$Codex_Ent_Fungoids_01_Polonium_Name;" => "Fungoida Setisis - White", 
			"$Codex_Ent_Fungoids_01_Tellurium_Name;" => "Fungoida Setisis - Yellow", 
			"$Codex_Ent_Fungoids_02_Cadmium_Name;" => "Fungoida Stabitis - Blue", 
			"$Codex_Ent_Fungoids_02_Mercury_Name;" => "Fungoida Stabitis - Green", 
			"$Codex_Ent_Fungoids_02_Molybdenum_Name;" => "Fungoida Stabitis - Magenta", 
			"$Codex_Ent_Fungoids_02_Tin_Name;" => "Fungoida Stabitis - Orange", 
			"$Codex_Ent_Fungoids_02_Tungsten_Name;" => "Fungoida Stabitis - Peach", 
			"$Codex_Ent_Fungoids_02_Niobium_Name;" => "Fungoida Stabitis - White", 
			"$Codex_Ent_Osseus_05_T_Name;" => "Osseus Cornibus - Emerald", 
			"$Codex_Ent_Osseus_05_TTS_Name;" => "Osseus Cornibus - Green", 
			"$Codex_Ent_Osseus_05_G_Name;" => "Osseus Cornibus - Grey", 
			"$Codex_Ent_Osseus_05_K_Name;" => "Osseus Cornibus - Indigo", 
			"$Codex_Ent_Osseus_05_A_Name;" => "Osseus Cornibus - Lime", 
			"$Codex_Ent_Osseus_05_Y_Name;" => "Osseus Cornibus - Maroon", 
			"$Codex_Ent_Osseus_05_F_Name;" => "Osseus Cornibus - Turquoise", 
			"$Codex_Ent_Osseus_02_Niobium_Name;" => "Osseus Discus - Aquamarine", 
			"$Codex_Ent_Osseus_02_Tin_Name;" => "Osseus Discus - Blue", 
			"$Codex_Ent_Osseus_02_Mercury_Name;" => "Osseus Discus - Lime", 
			"$Codex_Ent_Osseus_02_Molybdenum_Name;" => "Osseus Discus - Peach", 
			"$Codex_Ent_Osseus_02_Tungsten_Name;" => "Osseus Discus - Red", 
			"$Codex_Ent_Osseus_02_Cadmium_Name;" => "Osseus Discus - White", 
			"$Codex_Ent_Osseus_01_T_Name;" => "Osseus Fractus - Emerald", 
			"$Codex_Ent_Osseus_01_TTS_Name;" => "Osseus Fractus - Green", 
			"$Codex_Ent_Osseus_01_G_Name;" => "Osseus Fractus - Grey", 
			"$Codex_Ent_Osseus_01_K_Name;" => "Osseus Fractus - Indigo", 
			"$Codex_Ent_Osseus_01_A_Name;" => "Osseus Fractus - Lime", 
			"$Codex_Ent_Osseus_01_Y_Name;" => "Osseus Fractus - Maroon", 
			"$Codex_Ent_Osseus_01_F_Name;" => "Osseus Fractus - Turquoise", 
			"$Codex_Ent_Osseus_06_T_Name;" => "Osseus Pellebantus - Emerald", 
			"$Codex_Ent_Osseus_06_TTS_Name;" => "Osseus Pellebantus - Green", 
			"$Codex_Ent_Osseus_06_G_Name;" => "Osseus Pellebantus - Grey", 
			"$Codex_Ent_Osseus_06_K_Name;" => "Osseus Pellebantus - Indigo", 
			"$Codex_Ent_Osseus_06_A_Name;" => "Osseus Pellebantus - Lime", 
			"$Codex_Ent_Osseus_06_Y_Name;" => "Osseus Pellebantus - Maroon", 
			"$Codex_Ent_Osseus_06_F_Name;" => "Osseus Pellebantus - Turquoise", 
			"$Codex_Ent_Osseus_04_Ruthenium_Name;" => "Osseus Pumice - Gold", 
			"$Codex_Ent_Osseus_04_Tellurium_Name;" => "Osseus Pumice - Green", 
			"$Codex_Ent_Osseus_04_Technetium_Name;" => "Osseus Pumice - Lime", 
			"$Codex_Ent_Osseus_04_Polonium_Name;" => "Osseus Pumice - Peach", 
			"$Codex_Ent_Osseus_04_Antimony_Name;" => "Osseus Pumice - White", 
			"$Codex_Ent_Osseus_04_Yttrium_Name;" => "Osseus Pumice - Yellow", 
			"$Codex_Ent_Osseus_03_T_Name;" => "Osseus Spiralis - Emerald", 
			"$Codex_Ent_Osseus_03_TTS_Name;" => "Osseus Spiralis - Green", 
			"$Codex_Ent_Osseus_03_G_Name;" => "Osseus Spiralis - Grey", 
			"$Codex_Ent_Osseus_03_K_Name;" => "Osseus Spiralis - Indigo", 
			"$Codex_Ent_Osseus_03_A_Name;" => "Osseus Spiralis - Lime", 
			"$Codex_Ent_Osseus_03_Y_Name;" => "Osseus Spiralis - Maroon", 
			"$Codex_Ent_Osseus_03_F_Name;" => "Osseus Spiralis - Turquoise", 
			"$Codex_Ent_Osseus_03_O_Name;" => "Osseus Spiralis - Yellow", 
			"$Codex_Ent_SphereEFGH_02_Name;" => "Prasinum Bioluminescent Anemone", 
			"$Codex_Ent_Recepta_03_Technetium_Name;" => "Recepta Conditivus - Aquamarine", 
			"$Codex_Ent_Recepta_03_Tellurium_Name;" => "Recepta Conditivus - Cyan", 
			"$Codex_Ent_Recepta_03_Yttrium_Name;" => "Recepta Conditivus - Green", 
			"$Codex_Ent_Recepta_03_Antimony_Name;" => "Recepta Conditivus - Lime", 
			"$Codex_Ent_Recepta_03_Polonium_Name;" => "Recepta Conditivus - White", 
			"$Codex_Ent_Recepta_03_Ruthenium_Name;" => "Recepta Conditivus - Yellow", 
			"$Codex_Ent_Recepta_02_Mercury_Name;" => "Recepta Deltahedronix - Cyan", 
			"$Codex_Ent_Recepta_02_Molybdenum_Name;" => "Recepta Deltahedronix - Gold", 
			"$Codex_Ent_Recepta_02_Cadmium_Name;" => "Recepta Deltahedronix - Lime", 
			"$Codex_Ent_Recepta_02_Niobium_Name;" => "Recepta Deltahedronix - Mulberry", 
			"$Codex_Ent_Recepta_02_Tin_Name;" => "Recepta Deltahedronix - Orange", 
			"$Codex_Ent_Recepta_02_Tungsten_Name;" => "Recepta Deltahedronix - Red", 
			"$Codex_Ent_Recepta_01_A_Name;" => "Recepta Umbrux - Amethyst", 
			"$Codex_Ent_Recepta_01_N_Name;" => "Recepta Umbrux - Emerald", 
			"$Codex_Ent_Recepta_01_Ae_Name;" => "Recepta Umbrux - Grey", 
			"$Codex_Ent_Recepta_01_O_Name;" => "Recepta Umbrux - Indigo", 
			"$Codex_Ent_Recepta_01_Y_Name;" => "Recepta Umbrux - Lime", 
			"$Codex_Ent_Recepta_01_M_Name;" => "Recepta Umbrux - Maroon", 
			"$Codex_Ent_Recepta_01_F_Name;" => "Recepta Umbrux - Mauve", 
			"$Codex_Ent_Recepta_01_L_Name;" => "Recepta Umbrux - Ocher", 
			"$Codex_Ent_Recepta_01_G_Name;" => "Recepta Umbrux - Orange", 
			"$Codex_Ent_Recepta_01_K_Name;" => "Recepta Umbrux - Red", 
			"$Codex_Ent_Recepta_01_TTS_Name;" => "Recepta Umbrux - Sage", 
			"$Codex_Ent_Recepta_01_T_Name;" => "Recepta Umbrux - Teal", 
			"$Codex_Ent_Recepta_01_B_Name;" => "Recepta Umbrux - Turquoise", 
			"$Codex_Ent_Recepta_01_D_Name;" => "Recepta Umbrux - Yellow", 
			"$Codex_Ent_SphereEFGH_03_Name;" => "Roseum Bioluminescent Anemone", 
			"$Codex_Ent_Seed_Name;" => "Roseum Brain Tree", 
			"$Codex_Ent_SphereEFGH_01_Name;" => "Rubeum Bioluminescent Anemone", 
			"$Codex_Ent_Stratum_04_F_Name;" => "Stratum Araneamus - Emerald", 
			"$Codex_Ent_Stratum_06_TTS_Name;" => "Stratum Cucumisis - Amethyst", 
			"$Codex_Ent_Stratum_06_F_Name;" => "Stratum Cucumisis - Emerald", 
			"$Codex_Ent_Stratum_06_M_Name;" => "Stratum Cucumisis - Green", 
			"$Codex_Ent_Stratum_06_T_Name;" => "Stratum Cucumisis - Grey", 
			"$Codex_Ent_Stratum_06_Y_Name;" => "Stratum Cucumisis - Indigo", 
			"$Codex_Ent_Stratum_06_K_Name;" => "Stratum Cucumisis - Lime", 
			"$Codex_Ent_Stratum_06_D_Name;" => "Stratum Cucumisis - Mauve", 
			"$Codex_Ent_Stratum_06_W_Name;" => "Stratum Cucumisis - Red", 
			"$Codex_Ent_Stratum_06_Ae_Name;" => "Stratum Cucumisis - Teal", 
			"$Codex_Ent_Stratum_06_L_Name;" => "Stratum Cucumisis - Turquoise", 
			"$Codex_Ent_Stratum_01_TTS_Name;" => "Stratum Excutitus - Amethyst", 
			"$Codex_Ent_Stratum_01_F_Name;" => "Stratum Excutitus - Emerald", 
			"$Codex_Ent_Stratum_01_M_Name;" => "Stratum Excutitus - Green", 
			"$Codex_Ent_Stratum_01_T_Name;" => "Stratum Excutitus - Grey", 
			"$Codex_Ent_Stratum_01_Y_Name;" => "Stratum Excutitus - Indigo", 
			"$Codex_Ent_Stratum_01_K_Name;" => "Stratum Excutitus - Lime", 
			"$Codex_Ent_Stratum_01_D_Name;" => "Stratum Excutitus - Mauve", 
			"$Codex_Ent_Stratum_01_W_Name;" => "Stratum Excutitus - Red", 
			"$Codex_Ent_Stratum_01_Ae_Name;" => "Stratum Excutitus - Teal", 
			"$Codex_Ent_Stratum_01_L_Name;" => "Stratum Excutitus - Turquoise", 
			"$Codex_Ent_Stratum_03_TTS_Name;" => "Stratum Laminamus - Amethyst", 
			"$Codex_Ent_Stratum_03_F_Name;" => "Stratum Laminamus - Emerald", 
			"$Codex_Ent_Stratum_03_M_Name;" => "Stratum Laminamus - Green", 
			"$Codex_Ent_Stratum_03_T_Name;" => "Stratum Laminamus - Grey", 
			"$Codex_Ent_Stratum_03_Y_Name;" => "Stratum Laminamus - Indigo", 
			"$Codex_Ent_Stratum_03_K_Name;" => "Stratum Laminamus - Lime", 
			"$Codex_Ent_Stratum_03_D_Name;" => "Stratum Laminamus - Mauve", 
			"$Codex_Ent_Stratum_03_W_Name;" => "Stratum Laminamus - Red", 
			"$Codex_Ent_Stratum_03_L_Name;" => "Stratum Laminamus - Turquoise", 
			"$Codex_Ent_Stratum_05_TTS_Name;" => "Stratum Limaxus - Amethyst", 
			"$Codex_Ent_Stratum_05_F_Name;" => "Stratum Limaxus - Emerald", 
			"$Codex_Ent_Stratum_05_M_Name;" => "Stratum Limaxus - Green", 
			"$Codex_Ent_Stratum_05_T_Name;" => "Stratum Limaxus - Grey", 
			"$Codex_Ent_Stratum_05_Y_Name;" => "Stratum Limaxus - Indigo", 
			"$Codex_Ent_Stratum_05_K_Name;" => "Stratum Limaxus - Lime", 
			"$Codex_Ent_Stratum_05_D_Name;" => "Stratum Limaxus - Mauve", 
			"$Codex_Ent_Stratum_05_Ae_Name;" => "Stratum Limaxus - Teal", 
			"$Codex_Ent_Stratum_05_L_Name;" => "Stratum Limaxus - Turquoise", 
			"$Codex_Ent_Stratum_02_TTS_Name;" => "Stratum Paleas - Amethyst", 
			"$Codex_Ent_Stratum_02_F_Name;" => "Stratum Paleas - Emerald", 
			"$Codex_Ent_Stratum_02_M_Name;" => "Stratum Paleas - Green", 
			"$Codex_Ent_Stratum_02_T_Name;" => "Stratum Paleas - Grey", 
			"$Codex_Ent_Stratum_02_Y_Name;" => "Stratum Paleas - Indigo", 
			"$Codex_Ent_Stratum_02_K_Name;" => "Stratum Paleas - Lime", 
			"$Codex_Ent_Stratum_02_D_Name;" => "Stratum Paleas - Mauve", 
			"$Codex_Ent_Stratum_02_W_Name;" => "Stratum Paleas - Red", 
			"$Codex_Ent_Stratum_02_Ae_Name;" => "Stratum Paleas - Teal", 
			"$Codex_Ent_Stratum_02_L_Name;" => "Stratum Paleas - Turquoise", 
			"$Codex_Ent_Stratum_07_TTS_Name;" => "Stratum Tectonicas - Amethyst", 
			"$Codex_Ent_Stratum_07_F_Name;" => "Stratum Tectonicas - Emerald", 
			"$Codex_Ent_Stratum_07_M_Name;" => "Stratum Tectonicas - Green", 
			"$Codex_Ent_Stratum_07_T_Name;" => "Stratum Tectonicas - Grey", 
			"$Codex_Ent_Stratum_07_Y_Name;" => "Stratum Tectonicas - Indigo", 
			"$Codex_Ent_Stratum_07_K_Name;" => "Stratum Tectonicas - Lime", 
			"$Codex_Ent_Stratum_07_D_Name;" => "Stratum Tectonicas - Mauve", 
			"$Codex_Ent_Stratum_07_W_Name;" => "Stratum Tectonicas - Red", 
			"$Codex_Ent_Stratum_07_L_Name;" => "Stratum Tectonicas - Turquoise", 
			"$Codex_Ent_Tubus_03_N_Name;" => "Tubus Cavas - Amethyst", 
			"$Codex_Ent_Tubus_03_B_Name;" => "Tubus Cavas - Emerald", 
			"$Codex_Ent_Tubus_03_F_Name;" => "Tubus Cavas - Grey", 
			"$Codex_Ent_Tubus_03_A_Name;" => "Tubus Cavas - Indigo", 
			"$Codex_Ent_Tubus_03_K_Name;" => "Tubus Cavas - Maroon", 
			"$Codex_Ent_Tubus_03_T_Name;" => "Tubus Cavas - Mauve", 
			"$Codex_Ent_Tubus_03_TTS_Name;" => "Tubus Cavas - Ocher", 
			"$Codex_Ent_Tubus_03_G_Name;" => "Tubus Cavas - Red", 
			"$Codex_Ent_Tubus_03_M_Name;" => "Tubus Cavas - Teal", 
			"$Codex_Ent_Tubus_03_L_Name;" => "Tubus Cavas - Turquoise", 
			"$Codex_Ent_Tubus_03_D_Name;" => "Tubus Cavas - Yellow", 
			"$Codex_Ent_Tubus_05_N_Name;" => "Tubus Compagibus - Amethyst", 
			"$Codex_Ent_Tubus_05_B_Name;" => "Tubus Compagibus - Emerald", 
			"$Codex_Ent_Tubus_05_F_Name;" => "Tubus Compagibus - Grey", 
			"$Codex_Ent_Tubus_05_A_Name;" => "Tubus Compagibus - Indigo", 
			"$Codex_Ent_Tubus_05_W_Name;" => "Tubus Compagibus - Lime", 
			"$Codex_Ent_Tubus_05_K_Name;" => "Tubus Compagibus - Maroon", 
			"$Codex_Ent_Tubus_05_T_Name;" => "Tubus Compagibus - Mauve", 
			"$Codex_Ent_Tubus_05_TTS_Name;" => "Tubus Compagibus - Ocher", 
			"$Codex_Ent_Tubus_05_G_Name;" => "Tubus Compagibus - Red", 
			"$Codex_Ent_Tubus_05_M_Name;" => "Tubus Compagibus - Teal", 
			"$Codex_Ent_Tubus_05_L_Name;" => "Tubus Compagibus - Turquoise", 
			"$Codex_Ent_Tubus_05_D_Name;" => "Tubus Compagibus - Yellow", 
			"$Codex_Ent_Tubus_04_N_Name;" => "Tubus Rosarium - Amethyst", 
			"$Codex_Ent_Tubus_04_B_Name;" => "Tubus Rosarium - Emerald", 
			"$Codex_Ent_Tubus_04_O_Name;" => "Tubus Rosarium - Green", 
			"$Codex_Ent_Tubus_04_F_Name;" => "Tubus Rosarium - Grey", 
			"$Codex_Ent_Tubus_04_A_Name;" => "Tubus Rosarium - Indigo", 
			"$Codex_Ent_Tubus_04_K_Name;" => "Tubus Rosarium - Maroon", 
			"$Codex_Ent_Tubus_04_T_Name;" => "Tubus Rosarium - Mauve", 
			"$Codex_Ent_Tubus_04_TTS_Name;" => "Tubus Rosarium - Ocher", 
			"$Codex_Ent_Tubus_04_G_Name;" => "Tubus Rosarium - Red", 
			"$Codex_Ent_Tubus_04_M_Name;" => "Tubus Rosarium - Teal", 
			"$Codex_Ent_Tubus_04_L_Name;" => "Tubus Rosarium - Turquoise", 
			"$Codex_Ent_Tubus_04_D_Name;" => "Tubus Rosarium - Yellow", 
			"$Codex_Ent_Tubus_02_N_Name;" => "Tubus Sororibus - Amethyst", 
			"$Codex_Ent_Tubus_02_B_Name;" => "Tubus Sororibus - Emerald", 
			"$Codex_Ent_Tubus_02_F_Name;" => "Tubus Sororibus - Grey", 
			"$Codex_Ent_Tubus_02_A_Name;" => "Tubus Sororibus - Indigo", 
			"$Codex_Ent_Tubus_02_K_Name;" => "Tubus Sororibus - Maroon", 
			"$Codex_Ent_Tubus_02_T_Name;" => "Tubus Sororibus - Mauve", 
			"$Codex_Ent_Tubus_02_TTS_Name;" => "Tubus Sororibus - Ocher", 
			"$Codex_Ent_Tubus_02_G_Name;" => "Tubus Sororibus - Red", 
			"$Codex_Ent_Tubus_02_M_Name;" => "Tubus Sororibus - Teal", 
			"$Codex_Ent_Tubus_02_L_Name;" => "Tubus Sororibus - Turquoise", 
			"$Codex_Ent_Tubus_02_D_Name;" => "Tubus Sororibus - Yellow", 
			"$Codex_Ent_Tussocks_08_M_Name;" => "Tussock Albata - Emerald", 
			"$Codex_Ent_Tussocks_08_K_Name;" => "Tussock Albata - Green", 
			"$Codex_Ent_Tussocks_08_G_Name;" => "Tussock Albata - Lime", 
			"$Codex_Ent_Tussocks_08_D_Name;" => "Tussock Albata - Maroon", 
			"$Codex_Ent_Tussocks_08_W_Name;" => "Tussock Albata - Orange", 
			"$Codex_Ent_Tussocks_08_Y_Name;" => "Tussock Albata - Red", 
			"$Codex_Ent_Tussocks_08_L_Name;" => "Tussock Albata - Sage", 
			"$Codex_Ent_Tussocks_08_T_Name;" => "Tussock Albata - Teal", 
			"$Codex_Ent_Tussocks_08_F_Name;" => "Tussock Albata - Yellow", 
			"$Codex_Ent_Tussocks_15_M_Name;" => "Tussock Capillum - Emerald", 
			"$Codex_Ent_Tussocks_15_K_Name;" => "Tussock Capillum - Green", 
			"$Codex_Ent_Tussocks_15_G_Name;" => "Tussock Capillum - Lime", 
			"$Codex_Ent_Tussocks_15_D_Name;" => "Tussock Capillum - Maroon", 
			"$Codex_Ent_Tussocks_15_Y_Name;" => "Tussock Capillum - Red", 
			"$Codex_Ent_Tussocks_15_L_Name;" => "Tussock Capillum - Sage", 
			"$Codex_Ent_Tussocks_15_T_Name;" => "Tussock Capillum - Teal", 
			"$Codex_Ent_Tussocks_15_F_Name;" => "Tussock Capillum - Yellow", 
			"$Codex_Ent_Tussocks_11_M_Name;" => "Tussock Caputus - Emerald", 
			"$Codex_Ent_Tussocks_11_K_Name;" => "Tussock Caputus - Green", 
			"$Codex_Ent_Tussocks_11_G_Name;" => "Tussock Caputus - Lime", 
			"$Codex_Ent_Tussocks_11_D_Name;" => "Tussock Caputus - Maroon", 
			"$Codex_Ent_Tussocks_11_Y_Name;" => "Tussock Caputus - Red", 
			"$Codex_Ent_Tussocks_11_L_Name;" => "Tussock Caputus - Sage", 
			"$Codex_Ent_Tussocks_11_T_Name;" => "Tussock Caputus - Teal", 
			"$Codex_Ent_Tussocks_11_F_Name;" => "Tussock Caputus - Yellow", 
			"$Codex_Ent_Tussocks_05_M_Name;" => "Tussock Catena - Emerald", 
			"$Codex_Ent_Tussocks_05_K_Name;" => "Tussock Catena - Green", 
			"$Codex_Ent_Tussocks_05_G_Name;" => "Tussock Catena - Lime", 
			"$Codex_Ent_Tussocks_05_D_Name;" => "Tussock Catena - Maroon", 
			"$Codex_Ent_Tussocks_05_Y_Name;" => "Tussock Catena - Red", 
			"$Codex_Ent_Tussocks_05_L_Name;" => "Tussock Catena - Sage", 
			"$Codex_Ent_Tussocks_05_T_Name;" => "Tussock Catena - Teal", 
			"$Codex_Ent_Tussocks_05_F_Name;" => "Tussock Catena - Yellow", 
			"$Codex_Ent_Tussocks_04_M_Name;" => "Tussock Cultro - Emerald", 
			"$Codex_Ent_Tussocks_04_K_Name;" => "Tussock Cultro - Green", 
			"$Codex_Ent_Tussocks_04_G_Name;" => "Tussock Cultro - Lime", 
			"$Codex_Ent_Tussocks_04_D_Name;" => "Tussock Cultro - Maroon", 
			"$Codex_Ent_Tussocks_04_W_Name;" => "Tussock Cultro - Orange", 
			"$Codex_Ent_Tussocks_04_Y_Name;" => "Tussock Cultro - Red", 
			"$Codex_Ent_Tussocks_04_L_Name;" => "Tussock Cultro - Sage", 
			"$Codex_Ent_Tussocks_04_T_Name;" => "Tussock Cultro - Teal", 
			"$Codex_Ent_Tussocks_04_F_Name;" => "Tussock Cultro - Yellow", 
			"$Codex_Ent_Tussocks_10_M_Name;" => "Tussock Divisa - Emerald", 
			"$Codex_Ent_Tussocks_10_K_Name;" => "Tussock Divisa - Green", 
			"$Codex_Ent_Tussocks_10_G_Name;" => "Tussock Divisa - Lime", 
			"$Codex_Ent_Tussocks_10_D_Name;" => "Tussock Divisa - Maroon", 
			"$Codex_Ent_Tussocks_10_Y_Name;" => "Tussock Divisa - Red", 
			"$Codex_Ent_Tussocks_10_L_Name;" => "Tussock Divisa - Sage", 
			"$Codex_Ent_Tussocks_10_T_Name;" => "Tussock Divisa - Teal", 
			"$Codex_Ent_Tussocks_10_F_Name;" => "Tussock Divisa - Yellow", 
			"$Codex_Ent_Tussocks_03_M_Name;" => "Tussock Ignis - Emerald", 
			"$Codex_Ent_Tussocks_03_K_Name;" => "Tussock Ignis - Green", 
			"$Codex_Ent_Tussocks_03_G_Name;" => "Tussock Ignis - Lime", 
			"$Codex_Ent_Tussocks_03_D_Name;" => "Tussock Ignis - Maroon", 
			"$Codex_Ent_Tussocks_03_W_Name;" => "Tussock Ignis - Orange", 
			"$Codex_Ent_Tussocks_03_Y_Name;" => "Tussock Ignis - Red", 
			"$Codex_Ent_Tussocks_03_L_Name;" => "Tussock Ignis - Sage", 
			"$Codex_Ent_Tussocks_03_T_Name;" => "Tussock Ignis - Teal", 
			"$Codex_Ent_Tussocks_03_F_Name;" => "Tussock Ignis - Yellow", 
			"$Codex_Ent_Tussocks_01_M_Name;" => "Tussock Pennata - Emerald", 
			"$Codex_Ent_Tussocks_01_K_Name;" => "Tussock Pennata - Green", 
			"$Codex_Ent_Tussocks_01_G_Name;" => "Tussock Pennata - Lime", 
			"$Codex_Ent_Tussocks_01_D_Name;" => "Tussock Pennata - Maroon", 
			"$Codex_Ent_Tussocks_01_W_Name;" => "Tussock Pennata - Orange", 
			"$Codex_Ent_Tussocks_01_Y_Name;" => "Tussock Pennata - Red", 
			"$Codex_Ent_Tussocks_01_L_Name;" => "Tussock Pennata - Sage", 
			"$Codex_Ent_Tussocks_01_T_Name;" => "Tussock Pennata - Teal", 
			"$Codex_Ent_Tussocks_01_F_Name;" => "Tussock Pennata - Yellow", 
			"$Codex_Ent_Tussocks_06_M_Name;" => "Tussock Pennatis - Emerald", 
			"$Codex_Ent_Tussocks_06_K_Name;" => "Tussock Pennatis - Green", 
			"$Codex_Ent_Tussocks_06_G_Name;" => "Tussock Pennatis - Lime", 
			"$Codex_Ent_Tussocks_06_D_Name;" => "Tussock Pennatis - Maroon", 
			"$Codex_Ent_Tussocks_06_Y_Name;" => "Tussock Pennatis - Red", 
			"$Codex_Ent_Tussocks_06_L_Name;" => "Tussock Pennatis - Sage", 
			"$Codex_Ent_Tussocks_06_T_Name;" => "Tussock Pennatis - Teal", 
			"$Codex_Ent_Tussocks_06_F_Name;" => "Tussock Pennatis - Yellow", 
			"$Codex_Ent_Tussocks_09_M_Name;" => "Tussock Propagito - Emerald", 
			"$Codex_Ent_Tussocks_09_K_Name;" => "Tussock Propagito - Green", 
			"$Codex_Ent_Tussocks_09_G_Name;" => "Tussock Propagito - Lime", 
			"$Codex_Ent_Tussocks_09_D_Name;" => "Tussock Propagito - Maroon", 
			"$Codex_Ent_Tussocks_09_Y_Name;" => "Tussock Propagito - Red", 
			"$Codex_Ent_Tussocks_09_L_Name;" => "Tussock Propagito - Sage", 
			"$Codex_Ent_Tussocks_09_T_Name;" => "Tussock Propagito - Teal", 
			"$Codex_Ent_Tussocks_09_F_Name;" => "Tussock Propagito - Yellow", 
			"$Codex_Ent_Tussocks_07_M_Name;" => "Tussock Serrati - Emerald", 
			"$Codex_Ent_Tussocks_07_K_Name;" => "Tussock Serrati - Green", 
			"$Codex_Ent_Tussocks_07_G_Name;" => "Tussock Serrati - Lime", 
			"$Codex_Ent_Tussocks_07_D_Name;" => "Tussock Serrati - Maroon", 
			"$Codex_Ent_Tussocks_07_Y_Name;" => "Tussock Serrati - Red", 
			"$Codex_Ent_Tussocks_07_L_Name;" => "Tussock Serrati - Sage", 
			"$Codex_Ent_Tussocks_07_T_Name;" => "Tussock Serrati - Teal", 
			"$Codex_Ent_Tussocks_07_F_Name;" => "Tussock Serrati - Yellow", 
			"$Codex_Ent_Tussocks_13_M_Name;" => "Tussock Stigmasis - Emerald", 
			"$Codex_Ent_Tussocks_13_K_Name;" => "Tussock Stigmasis - Green", 
			"$Codex_Ent_Tussocks_13_G_Name;" => "Tussock Stigmasis - Lime", 
			"$Codex_Ent_Tussocks_13_D_Name;" => "Tussock Stigmasis - Maroon", 
			"$Codex_Ent_Tussocks_13_Y_Name;" => "Tussock Stigmasis - Red", 
			"$Codex_Ent_Tussocks_13_L_Name;" => "Tussock Stigmasis - Sage", 
			"$Codex_Ent_Tussocks_13_T_Name;" => "Tussock Stigmasis - Teal", 
			"$Codex_Ent_Tussocks_13_F_Name;" => "Tussock Stigmasis - Yellow", 
			"$Codex_Ent_Tussocks_12_M_Name;" => "Tussock Triticum - Emerald", 
			"$Codex_Ent_Tussocks_12_K_Name;" => "Tussock Triticum - Green", 
			"$Codex_Ent_Tussocks_12_G_Name;" => "Tussock Triticum - Lime", 
			"$Codex_Ent_Tussocks_12_D_Name;" => "Tussock Triticum - Maroon", 
			"$Codex_Ent_Tussocks_12_Y_Name;" => "Tussock Triticum - Red", 
			"$Codex_Ent_Tussocks_12_L_Name;" => "Tussock Triticum - Sage", 
			"$Codex_Ent_Tussocks_12_T_Name;" => "Tussock Triticum - Teal", 
			"$Codex_Ent_Tussocks_12_F_Name;" => "Tussock Triticum - Yellow", 
			"$Codex_Ent_Tussocks_02_M_Name;" => "Tussock Ventusa - Emerald", 
			"$Codex_Ent_Tussocks_02_K_Name;" => "Tussock Ventusa - Green", 
			"$Codex_Ent_Tussocks_02_G_Name;" => "Tussock Ventusa - Lime", 
			"$Codex_Ent_Tussocks_02_D_Name;" => "Tussock Ventusa - Maroon", 
			"$Codex_Ent_Tussocks_02_W_Name;" => "Tussock Ventusa - Orange", 
			"$Codex_Ent_Tussocks_02_Y_Name;" => "Tussock Ventusa - Red", 
			"$Codex_Ent_Tussocks_02_L_Name;" => "Tussock Ventusa - Sage", 
			"$Codex_Ent_Tussocks_02_T_Name;" => "Tussock Ventusa - Teal", 
			"$Codex_Ent_Tussocks_02_F_Name;" => "Tussock Ventusa - Yellow", 
			"$Codex_Ent_Tussocks_14_M_Name;" => "Tussock Virgam - Emerald", 
			"$Codex_Ent_Tussocks_14_K_Name;" => "Tussock Virgam - Green", 
			"$Codex_Ent_Tussocks_14_G_Name;" => "Tussock Virgam - Lime", 
			"$Codex_Ent_Tussocks_14_D_Name;" => "Tussock Virgam - Maroon", 
			"$Codex_Ent_Tussocks_14_L_Name;" => "Tussock Virgam - Sage", 
			"$Codex_Ent_Tussocks_14_T_Name;" => "Tussock Virgam - Teal", 
			"$Codex_Ent_Tussocks_14_F_Name;" => "Tussock Virgam - Yellow", 
			"$Codex_Ent_Ingensradices_Unicus_Name;" => "Radicoida Unica", 
			_ => "Unknown", 
		};
		if (1 == 0)
		{
		}
		return result;
	}
}
