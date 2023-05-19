using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Runtime.InteropServices;

/// <summary>
/// Auteur : #TP4 ZACHARY
/// Script qui permet d'enregistrer les données du joueur dans un fichier .tim
/// On enregistre le nom du joueur par défaut ainsi que son score dans une liste
/// On trie enfin celle-ci pour avoir le top3 des meilleurs joueurs sur la machine locale
/// </summary>
[CreateAssetMenu(menuName = "TIM/Sauvegarde", fileName = "Sauvegarde")]
public class SOSauvegarde : ScriptableObject
{

    [Header("Textes")]
    [SerializeField] private TextMeshProUGUI _texteArgent;
    [SerializeField] private TextMeshProUGUI _textePtsArgent;
    [SerializeField] private TextMeshProUGUI _texteNiveau;
    [SerializeField] private TextMeshProUGUI _textePtsNiveau;
    [SerializeField] private TextMeshProUGUI _texteVie;
    [SerializeField] private TextMeshProUGUI _textePtsVie;
    [SerializeField] private TextMeshProUGUI _texteScoreFinal;
    [SerializeField] private TextMeshProUGUI _texteNomJoueur;
    [SerializeField] private TextMeshProUGUI _texteTopScore;

    [Header("Données Personnage")]
    [SerializeField] private SOPerso _donneesPerso;
    public SOPerso DonneesPerso => _donneesPerso;

    [Header("Paramètres de jeu")]
    public bool scoreFinPartieDansTop3;

    [Header("Liste des joueurs")]
    [SerializeField] private List<Joueur> _listeJoueurs = new List<Joueur>();

    private int joueurIndex;
    public int JoueurIndex { get => joueurIndex; set => joueurIndex = value; }

    // #TP4 ZACHARY
    // Permet de développer le jeu en WebGL
    [DllImport("__Internal")]
    static extern void SynchroniserWebGL();

    // #TP4 ZACHARY 
    // Classe pour stocker les informations du joueur
    [System.Serializable]
    public class Joueur
    {
        public string nom;
        public int score;
    }

    // #TP4 ZACHARY
    // On associe notre fichier dans le dossier Donnees
    //[SerializeField] string _fichier = "/Scripts/Donnees/Data.tim";

    // #TP4 ZACHARY
    // On utilise const car le multiplicateur ne peut pas changer
    // On évite le hard code
    private const int PTSPARNIVEAU = 1200;
    private const int PTSPARVIE = 100;
    private const int PTSPARARGENT = 50;

    string _fichier = "Demo.tim";

    /// <summary>
    /// #TP4 ZACHARY
    /// Méthode qui traite notre fichier en créant un objet Joueur et en l'ajoutant dans notre liste de joueur
    /// Elle trie la liste par score décroissant afin d'avoir notre top 3 des meilleurs joueurs
    /// On finit par enregistrer la liste en donnée JSON
    /// </summary>
    public void TraiterFichier()
    {
        // Création de l'objet joueur
        Joueur joueur = new Joueur
        {
            nom = _texteNomJoueur.text,
            score = _donneesPerso.niveau * PTSPARNIVEAU + _donneesPerso.vie * PTSPARVIE + _donneesPerso.argent * PTSPARARGENT
        };

        // Ajout du joueur à la liste
        _listeJoueurs.Add(joueur);

        // Tri de la liste par score décroissant
        _listeJoueurs.Sort((j1, j2) => j2.score.CompareTo(j1.score));

        // Enregistrement dans un fichier JSON (dans notre liste)
        string json = JsonUtility.ToJson(_listeJoueurs);
        string fichierEtChemin = Application.persistentDataPath + "/" + _fichier;
        File.WriteAllText(fichierEtChemin, json);
        // On Vérifie si on exécute le jeu on WebGL, si oui,
        // On apelle la méthode suivante
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            SynchroniserWebGL();
            Debug.Log("SynchroniserWebGL");
        }

        if (File.Exists(fichierEtChemin))
        {
            // Charger les données existantes du fichier dans la liste
            string contenu = File.ReadAllText(fichierEtChemin);
            JsonUtility.FromJsonOverwrite(contenu, this);
        }

        // Convertir la liste en JSON
        json = JsonUtility.ToJson(this);

        // Écrire le JSON dans le fichier
        File.WriteAllText(fichierEtChemin, json);
        Debug.Log(fichierEtChemin);

        // On assure la sauvegarde des données JSON, et on évite l'écrasement des données
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }


    /// <summary>
    /// #TP4 ZACHARY 
    /// Méthode qui affiche les meilleurs scores dans un panneau de texte en triant la liste des joueurs
    /// du plus grand au plus petit score et en affichant uniquement les trois premiers joueurs.
    /// Si notre score fait parti du top3, il sera en jaune
    /// </summary>
    /// <param name="texteTopScore"> On le passe en param pour pouvoir l'afficher dans l'autre script</param>
    public void AfficherTopScores(TextMeshProUGUI texteTopScore, int scoreFinPartie)
    {
        // On trie notre liste de joueurs du plus petit au plus grand
        _listeJoueurs.Sort((joueur1, joueur2) => joueur2.score.CompareTo(joueur1.score));

        // #synthese_ZACHARY
        // Permet de trouver l'index du joueur qui a le score le plus proche du score de fin de partie
        joueurIndex = 0;
        // On parcourt la liste de joueurs
        // On s'arrête dès qu'on trouve un joueur qui a un score plus petit que le score de fin de partie
        for (int i = 0; i < _listeJoueurs.Count; i++)
        {
            // On récupère le joueur à l'index i
            Joueur joueur = _listeJoueurs[i];
            // On vérifie si le score du joueur est plus petit que le score de fin de partie
            if (joueur.score <= scoreFinPartie)
            {
                // On met à jour l'index du joueur
                joueurIndex = i;
                break;
            }
        }

        // Titre de notre tableau du top3
        string texteAffiche = "";
        scoreFinPartieDansTop3 = false; // Variable pour vérifier si le score de fin de partie est dans le top 3
                                        // On compte seulement 3 fois pour avoir notre top3
        for (int i = 0; i < 3 && i < _listeJoueurs.Count; i++)
        {
            Joueur joueur = _listeJoueurs[i];
            string nom = joueur.nom;
            int score = joueur.score;

            // Vérification si le score de fin de partie est dans le top 3
            if (scoreFinPartie >= score && !scoreFinPartieDansTop3)
            {
                texteAffiche += "<color=yellow>"; // Ajout de la couleur jaune
                scoreFinPartieDansTop3 = true; // Mettre à jour la variable pour indiquer que le score de fin de partie est dans le top 3
            }

            texteAffiche += "Score : " + score + "\n"; // On fait +1 pour pas commencer à 0

            if (scoreFinPartie >= score && scoreFinPartieDansTop3)
            {
                texteAffiche += "</color>"; // Fin de la couleur jaune
            }
        }
        // On l'affiche dans notre panneau du top3
        texteTopScore.text = texteAffiche;
        Debug.Log(texteAffiche);
    }
}