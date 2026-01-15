using System.Collections.Generic;
using Verse;

namespace FreeWill
{
    /// <summary>
    /// Registry for managing work type priority calculation strategies.
    /// Provides centralized access to all strategy implementations.
    /// </summary>
    public static class WorkTypeStrategyRegistry
    {
        private static readonly Dictionary<string, IWorkTypeStrategy> strategies = new Dictionary<string, IWorkTypeStrategy>();
        private static IWorkTypeStrategy defaultStrategy;
        private static bool initialized = false;

        /// <summary>
        /// Initializes the registry with all available strategies.
        /// This is called automatically when first accessed.
        /// </summary>
        private static void Initialize()
        {
            if (initialized)
            {
                return;
            }

            // Don't initialize until the game and defs are fully loaded
            if (Current.Game == null || DefDatabase<WorkTypeDef>.DefCount == 0)
            {
                return;
            }
            try
            {
                // Register all strategy implementations
                RegisterStrategy(new FirefighterStrategy());
                RegisterStrategy(new PatientStrategy());
                RegisterStrategy(new DoctorStrategy());
                RegisterStrategy(new PatientBedRestStrategy());
                RegisterStrategy(new ChildcareStrategy());
                RegisterStrategy(new BasicWorkerStrategy());
                RegisterStrategy(new WardenStrategy());
                RegisterStrategy(new HandlingStrategy());
                RegisterStrategy(new CookingStrategy());
                RegisterStrategy(new HuntingStrategy());
                RegisterStrategy(new ConstructionStrategy());
                RegisterStrategy(new GrowingStrategy());
                RegisterStrategy(new MiningStrategy());
                RegisterStrategy(new PlantCuttingStrategy());
                RegisterStrategy(new SmithingStrategy());
                RegisterStrategy(new TailoringStrategy());
                RegisterStrategy(new ArtStrategy());
                RegisterStrategy(new CraftingStrategy());
                RegisterStrategy(new HaulingStrategy());
                RegisterStrategy(new CleaningStrategy());
                RegisterStrategy(new ResearchingStrategy());

                // Only register modded work type strategies if the work type exists
                if (DefDatabase<WorkTypeDef>.GetNamedSilentFail("HaulingUrgent") != null)
                {
                    RegisterStrategy(new HaulingUrgentStrategy());
                }

                // Handle default strategy separately since it has null WorkType
                defaultStrategy = new DefaultWorkTypeStrategy();

                initialized = true;
            }
            catch (System.Exception ex)
            {
                if (Prefs.DevMode)
                {
                    Log.Error($"Free Will: Failed to initialize WorkTypeStrategyRegistry: {ex}");
                }
            }
        }

        /// <summary>
        /// Registers a strategy for a specific work type.
        /// </summary>
        /// <param name="strategy">The strategy to register.</param>
        private static void RegisterStrategy(IWorkTypeStrategy strategy)
        {
            if (strategy?.WorkType?.defName == null)
            {
                if (Prefs.DevMode)
                {
                    Log.Warning($"Free Will: Failed to register strategy {strategy?.GetType()?.Name ?? "unknown"} - WorkType is null");
                }
                return;
            }
            strategies[strategy.WorkType.defName] = strategy;
        }

        // Complex Jobs parent mapping, lets just make them inherit default jobs
        private static readonly Dictionary<string, string> fsfParentMapping = new Dictionary<string, string>()
        {
            { "FSFNurse", "Doctor" },
            { "FSFSurgeon", "Doctor" },
            
            { "FSFTraining", "Handling" },
            { "FSFTaming", "Handling" },
            { "FSFSlaughter", "Handling" },
            
            { "FSFRepair", "Construction" },
            { "FSFDeconstruct", "Construction" },
            
            { "FSFHarvesting", "Growing" },
            { "FSFPruning", "PlantCutting" },
            
            { "FSFDrilling", "Mining" },
            
            { "FSFDrugs", "Crafting" },
            { "FSFMachining", "Crafting" },
            { "FSFFabrication", "Crafting" },
            { "FSFRefining", "Crafting" },
            { "FSFProduction", "Crafting" },
            { "FSFSmelt", "Smithing" },
            { "FSFStoneCut", "Smithing" },
            { "FSFButcher", "Cooking" },
            
            { "FSFRearming", "Hauling" },
            { "FSFTransport", "Hauling" },
            { "FSFCremating", "Hauling" },
            { "FSFDeliver", "Hauling" },
            { "FSFHauling", "Hauling" },
            
            { "FSFScan", "Research" },
        };

        /// <summary>
        /// Gets the strategy for the specified work type.
        /// If no specific strategy exists, returns the default strategy.
        /// </summary>
        /// <param name="workTypeDef">The work type to get a strategy for.</param>
        /// <returns>The appropriate strategy instance.</returns>
        public static IWorkTypeStrategy GetStrategy(WorkTypeDef workTypeDef)
        {
            Initialize();

            if (strategies.TryGetValue(workTypeDef.defName, out IWorkTypeStrategy strategy))
            {
                return strategy;
            }

            if (fsfParentMapping.TryGetValue(workTypeDef.defName, out string parentDefName))
            {
                if (strategies.TryGetValue(parentDefName, out IWorkTypeStrategy parentStrategy))
                {
                    return parentStrategy;
                }
            }

            // Fallback to default strategy
            return defaultStrategy;
        }

        /// <summary>
        /// Gets all registered strategies.
        /// </summary>
        /// <returns>Collection of all registered strategies.</returns>
        public static IEnumerable<IWorkTypeStrategy> GetAllStrategies()
        {
            Initialize();
            List<IWorkTypeStrategy> allStrategies = new List<IWorkTypeStrategy>(strategies.Values);
            if (defaultStrategy != null)
            {
                allStrategies.Add(defaultStrategy);
            }
            return allStrategies;
        }
    }
}
