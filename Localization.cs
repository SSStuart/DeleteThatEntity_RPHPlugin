using Rage;
using System.Collections.Generic;
using System.Globalization;

namespace DeleteThatEntity
{
    public class Localization
    {
        private readonly string locale;
        private readonly Dictionary<string, Dictionary<string, string>> strings = new Dictionary<string, Dictionary<string, string>>
        {
            {"en",
                new Dictionary<string, string>{ 
                    { "updateAvailable", "~y~Update available!" },
                    { "entitySelected", "Entity ~b~:selectedEntity~w~ selected" },
                    { "selectionConfirmation", "Press ~y~Delete~w~ to delete this entity, or ~y~Enter~w~ to cancel" },
                    { "nothingFound", "~o~Nothing found" },
                    { "unableToDelete", "~o~Unable to delete this entity" },
                    { "entityDeleted", "~g~Entity deleted" }
                }
            },
            {"fr",
                new Dictionary<string, string>{
                    { "updateAvailable", "~y~Mise à jour disponible !" },
                    { "entitySelected", "Entité ~b~:selectedEntity~w~ sélectionnée" },
                    { "selectionConfirmation", "Appuyez sur ~y~Suppr~w~ pour confirmer, ou ~y~Entrer~w~ pour annuler" },
                    { "nothingFound", "~o~Aucune entité trouvée" },
                    { "unableToDelete", "~o~Impossible de supprimer cette entité" },
                    { "entityDeleted", "~g~Entité supprimée" },
                }
            }
        };

        public Localization(string locale = "auto")
        {
            if (locale == "auto")
                locale = CultureInfo.InstalledUICulture.Name.Split('-')[0];
            else
                locale = locale.Split('-')[0];

            if (!strings.ContainsKey(locale))
                locale = "en";
            Game.LogTrivial($"Localization: Using locale '{locale.ToUpper()}'");

            this.locale = locale;
        }

        public string GetString(string key, params (string key, object value)[] replace)
        {
            string localizedString;
            if (strings[this.locale].ContainsKey(key))
                localizedString = strings[this.locale][key];
            else
            {
                localizedString = key;
                Game.LogTrivial($"Localization: Missing translation for key '{key}'");
            }

            foreach (var replacement in replace)
            {
                localizedString = localizedString.Replace($":{replacement.key}", replacement.value?.ToString() ?? "");
            }

            return localizedString;
        }
    }
}
