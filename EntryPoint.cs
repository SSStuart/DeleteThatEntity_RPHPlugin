using Rage;
using System.Reflection;

[assembly: Rage.Attributes.Plugin("DeleteThatEntity", Description = "A simple plugin allowing to remove most entity from the world.", Author = "SSStuart")]


namespace DeleteThatEntityPlugin
{
    public static class EntryPoint
    {
        public static string pluginName = "DeleteThatEntity";
        public static string pluginVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static void Main()
        {
            Game.LogTrivial($"{pluginName} Plugin v{pluginVersion} has been loaded.");

            // Check for updates
            bool updateAvailable = false;
            System.Threading.Tasks.Task.Run(async () =>
            {
                updateAvailable = await UpdateChecker.CheckUpdate();
            });

            GameFiber.StartNew(delegate
            {
                bool entityMarked = false;
                Entity selectedEntity = null;

                while (true)
                {
                    GameFiber.Yield();

                    if (updateAvailable)
                    {
                        UpdateChecker.DisplayUpdateNotification();
                        updateAvailable = false;
                    }

                    // If the player is aiming and the Delete key is pressed, mark the entity for deletion
                    if (!entityMarked && Game.IsKeyDown(System.Windows.Forms.Keys.Delete) && Game.LocalPlayer.IsFreeAiming)
                    {
                        selectedEntity = Game.LocalPlayer.GetFreeAimingTarget();
                        if (selectedEntity != null && selectedEntity.IsValid())
                        {
                            selectedEntity.Opacity = 0.5f;
                            entityMarked = true;

                            Game.DisplaySubtitle($"Entity ~b~{selectedEntity.Model.Name} ~w~selected");
                            Game.DisplayHelp("Press ~y~Delete~w~ to delete this entity, or ~y~Enter~w~ to cancel");
                            Game.LogTrivial($"[{pluginName}] Entity marked for deletion: {selectedEntity.Model.Name}");
                        }
                        else
                            Game.DisplaySubtitle("~o~Nothing found", 1000);

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
                            Game.DisplaySubtitle("~o~Unable to delete this entity", 1000);
                            selectedEntity.Opacity = 1f;
                            Game.LogTrivial($"[{pluginName}] Unable to delete entity: {selectedEntity.Model.Name}");
                        }
                        else
                        {
                            Game.DisplaySubtitle("~g~Entity deleted", 1000);
                            Game.LogTrivial($"[{pluginName}] Entity deleted");
                            }
                        }
                        else
                            Game.LogTrivial($"[{pluginName}] Entity marked for deletion does not exist anymore");
                        entityMarked = false;
                    }

                    // If the entity is marked for deletion and the enter key is pressed, cancel the deletion
                    if (entityMarked && Game.IsKeyDown(System.Windows.Forms.Keys.Enter))
                    {
                        entityMarked = false;
                        selectedEntity.Opacity = 1f;
                        Game.LogTrivial($"[{pluginName}] Entity deletion canceled for: {selectedEntity.Model.Name}");
                    }
                }
            });
        }
    }
}
