using Discord.Interactions;

namespace Discord.Addons {
    /// <summary>
    /// Used to create custom preconditions in runtime easily
    /// </summary>
    public class CustomPreconditionAttribute : PreconditionAttribute {
        public CustomPreconditionAttribute(PreconditionDelegate func) {
            _precondition = func;
        }

        public delegate Task<PreconditionResult> PreconditionDelegate(
            IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services);

        private readonly PreconditionDelegate _precondition;

        public override async Task<PreconditionResult> CheckRequirementsAsync(
            IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services) {
            return await _precondition(context, commandInfo, services);
        }
    }
}
