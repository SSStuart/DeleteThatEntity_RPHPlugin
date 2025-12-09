using DeleteThatEntity;
using Rage;
using System.Reflection;

[assembly: Rage.Attributes.Plugin("DeleteThatEntity", Description = "A simple plugin allowing to remove most entity from the world.", Author = "SSStuart", PrefersSingleInstance = true, SupportUrl = "https://ssstuart.net/discord")]


namespace DeleteThatEntityPlugin
{
    public static class EntryPoint
    {
        public static string pluginName = "DeleteThatEntity";
        public static string pluginVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static Localization l10n = new Localization();

        public static void Main()
        {
            Game.LogTrivial($"{pluginName} plugin v{pluginVersion} has been loaded.");

            UpdateChecker.CheckForUpdates();

            GameFiber.StartNew(delegate
            {
                bool entityMarked = false;
                Entity selectedEntity = null;

                while (true)
                {
                    GameFiber.Yield();

                    // If the player is aiming and the Delete key is pressed, mark the entity for deletion
                    if (!entityMarked && Game.IsKeyDown(System.Windows.Forms.Keys.Delete) && Game.LocalPlayer.IsFreeAiming)
                    {
                        selectedEntity = Game.LocalPlayer.GetFreeAimingTarget();
                        if (selectedEntity != null && selectedEntity.IsValid())
                        {
                            selectedEntity.Opacity = 0.5f;
                            entityMarked = true;

                            Game.DisplaySubtitle(l10n.GetString("entitySelected", ("selectedEntity", selectedEntity.Model.Name)));
                            Game.DisplayHelp(l10n.GetString("selectionConfirmation"));
                            Game.LogTrivial($"Entity marked for deletion: {selectedEntity.Model.Name}");
                        }
                        else
                            Game.DisplaySubtitle(l10n.GetString("nothingFound"), 1000);

                        // Waiting for the key to be released
                        while (Game.IsKeyDown(System.Windows.Forms.Keys.Delete))
                            GameFiber.Yield();
                    }

                    // If the entity is marked for deletion and the Delete key is pressed again, try deleting the entity
                    if (entityMarked && Game.IsKeyDown(System.Windows.Forms.Keys.Delete))
                    {
                        if (selectedEntity.Exists())
                        {
                            selectedEntity.Delete();
                        // If the entity still exist, show a message saying that the deletion as failed
                        if (selectedEntity.Exists())
                        {
                            Game.DisplaySubtitle(l10n.GetString("unableToDelete"), 1000);
                            selectedEntity.Opacity = 1f;
                            Game.LogTrivial($"Unable to delete entity: {selectedEntity.Model.Name}");
                        }
                        else
                        {
                            Game.DisplaySubtitle(l10n.GetString("entityDeleted"), 1000);
                            Game.LogTrivial($"Entity deleted");
                            }
                        }
                        else
                            Game.LogTrivial($"Entity marked for deletion does not exist anymore");
                        entityMarked = false;
                    }

                    // If the entity is marked for deletion and the enter key is pressed, cancel the deletion
                    if (entityMarked && Game.IsKeyDown(System.Windows.Forms.Keys.Enter))
                    {
                        entityMarked = false;
                        selectedEntity.Opacity = 1f;
                        Game.LogTrivial($"Entity deletion canceled for: {selectedEntity.Model.Name}");
                    }
                }
            });
        }
    }
}
