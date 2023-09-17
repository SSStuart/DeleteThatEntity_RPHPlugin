using Rage;

[assembly: Rage.Attributes.Plugin("DeleteThatEntity Plugin", Description = "A simple plugin allowing to remove most entity from the world.", Author = "SSStuart")]


namespace DeleteThatEntityPlugin
{
    public static class EntryPoint
    {
        public static void Main()
        {
            Game.LogTrivial("DeleteThatEntity Plugin loaded.");

            GameFiber.StartNew(delegate
            {
                bool entityMarked = false;
                Entity selectedEntity = null;

                while (true)
                {
                    GameFiber.Yield();

                    if (!entityMarked && Game.IsKeyDown(System.Windows.Forms.Keys.Delete) && Game.LocalPlayer.IsFreeAiming)
                    {
                        selectedEntity = Game.LocalPlayer.GetFreeAimingTarget();
                        if (selectedEntity != null && selectedEntity.IsValid()) {
                            selectedEntity.Opacity = 0.5f;
                            entityMarked = true;

                            Game.DisplaySubtitle("Entity ~b~" + selectedEntity.Model.Name + " ~w~selected");
                            Game.DisplayHelp("Press ~y~Delete~w~ to delete this entity, or ~y~Enter~w~ to cancel");
                            Game.LogTrivial("[DeleteThatEntity] Entity marked for deletion: " + selectedEntity.Model.Name);
                        }  
                        else
                            Game.DisplaySubtitle("~o~Nothing found", 1000);

                        while (Game.IsKeyDown(System.Windows.Forms.Keys.Delete))
                        {
                            GameFiber.Yield();
                        }
                    }

                    if (entityMarked && Game.IsKeyDown(System.Windows.Forms.Keys.Delete))
                    {
                        if (selectedEntity.Exists())
                        {
                            selectedEntity.Delete();
                            if (selectedEntity.Exists())
                            {
                                Game.DisplaySubtitle("~o~Unable to delete this entity", 1000);
                                selectedEntity.Opacity = 1f;
                                Game.LogTrivial("[DeleteThatEntity] Unable to delete entity: " + selectedEntity.Model.Name);
                            }
                            else
                            {
                                Game.DisplaySubtitle("~g~Entity deleted", 1000);
                                Game.LogTrivial("[DeleteThatEntity] Entity deleted");
                            }
                        }
                        else
                        {
                            
                            Game.LogTrivial("[DeleteThatEntity] Entity marked for deletion does not exist anymore");
                        }
                        entityMarked = false;

                    }

                    if (entityMarked && Game.IsKeyDown(System.Windows.Forms.Keys.Enter))
                    {
                        entityMarked = false;
                        selectedEntity.Opacity = 1f;
                        Game.LogTrivial("[DeleteThatEntity] Entity deletion canceled for: " + selectedEntity.Model.Name);
                    }
                }
            });
        }
    }
}