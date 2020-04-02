using System;
using JetBrains.Annotations;

namespace DaemonStageDispatcher
{
    public interface IDaemonStage
    {
        /**
         * Name of stage. Every stage has its own unique name.
         */
        [NotNull] string Name { get; }

        /**
         * Names of the stages that should be executed before this stage.
         * The list can be empty.
         * The list cannot be changed during execution.
         */
        [NotNull] string[] StagesBefore { get; }

        /**
         * Names of the stages that should be executed after this stage.
         * The list can be empty.
         * The list cannot be changed during execution.
         */
        [NotNull] string[] StagesAfter { get; }

        /**
         * Executes code analysis
         * <param name="sourceFile"> File to analyze </param>
         * <param name="checkForInterrupt"> Function to check for interrupts. Stage should check result of this function periodically
         *  and stop its execution as soon as possible after the function returns "true".</param>
         */
        void Execute(IPsiSourceFile sourceFile, Func<bool> checkForInterrupt);
    }
    public interface IPsiSourceFile { }
}
