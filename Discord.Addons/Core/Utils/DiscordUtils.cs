using Discord.Interactions;
using System.Reflection;
using JetBrains.Annotations;

namespace Discord.Addons.Utils {
    [PublicAPI]
    public static class DiscordUtils {
        public static readonly Assembly DiscordInteractionsAssembly = typeof(InteractionModuleBase).Assembly;

        #region Emotes

        public static bool TryParseEmote(string? str, out IEmote? emote) {
            emote = null;
            if (str == null) return false;
            if (Emote.TryParse(str, out var em)) {
                emote = em;
                return true;
            }
            if (Emoji.TryParse(str, out var ej)) {
                emote = ej;
                return true;
            }
            return false;
        }

        #endregion

        #region Modal

        public static void BuildModal(ModalBuilder builder, Type type) {
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToArray();
            foreach (var prop in props) {
                var attrs = prop.GetCustomAttributes();
                var textBuilder = new TextInputBuilder();
                var notDefault = false;
                foreach (var attr in attrs) {
                    switch (attr) {
                        case ModalTextInputAttribute modalInput:
                            textBuilder
                                .WithCustomId(modalInput.CustomId)
                                .WithMaxLength(modalInput.MaxLength)
                                .WithMinLength(modalInput.MinLength)
                                .WithPlaceholder(modalInput.Placeholder)
                                .WithStyle(modalInput.Style)
                                .WithValue(modalInput.InitialValue);
                            notDefault = true;
                            break;
                        case InputLabelAttribute modalInput:
                            textBuilder.WithLabel(modalInput.Label);
                            notDefault = true;
                            break;
                        case RequiredInputAttribute modalInput:
                            textBuilder.WithRequired(modalInput.IsRequired);
                            notDefault = true;
                            break;
                    }
                }
                if (!notDefault) continue;
                textBuilder.Label ??= prop.Name;
                builder.AddTextInput(textBuilder);
            }
            builder.WithTitle(type.Name);
        }

        #endregion

        #region Commands

        private static readonly MethodInfo _buildSlashCommandMethodInfo = 
            DiscordInteractionsAssembly.GetType("Discord.Interactions.Builders.ModuleClassBuilder")!
            .GetMethod("BuildSlashCommand", ReflectionUtils.StaticFlags)!;

        public static void BuildSlashCommand(Interactions.Builders.SlashCommandBuilder builder, 
            Func<IServiceProvider, IInteractionModuleBase> createInstance, 
            MethodInfo methodInfo, InteractionService commandService, IServiceProvider services) {
            _buildSlashCommandMethodInfo.Invoke(null, new object[] { 
                builder, createInstance, methodInfo, commandService, services });
        }

        #endregion

        #region Embeds

        public static Embed BuildErrorEmbed(Exception exception) => 
            BuildErrorEmbed(exception.ToString(), true);
        
        public static Embed BuildErrorEmbed(string error, bool internalError = false) => new EmbedBuilder()
            .WithTitle($"❌ Execution error! {(internalError ? "(Internal)" : string.Empty)}")
            .WithColor(Color.Red)
            .WithDescription(error)
            .Build();
        
        #endregion
    }
}
