using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Classe qui créé des nouvelles salles grâce à notre script Salle et fait une bordure extérieur(x,y)
/// Auteurs du code: Zachary Chandonnet 
/// Auteur des commentaires: Zachary Chandonnet
/// </summary>

public class Niveau : MonoBehaviour
{
     [Header("Tilemap")]
    [SerializeField] private Tilemap _tilemap;
    public Tilemap tileMap => _tilemap;
    [SerializeField] private Vector2Int _taille;

    [Header("Salles")]
    [SerializeField] private Salle[] _tSallesModeles;
    [SerializeField] private Salle _salle;

    [Header("Tuiles et Bonus")]
    [SerializeField] private TileBase _tuileModele;
    [SerializeField] private Bonus[] _tBonusModeles;
    [SerializeField] private Bonus2[] _tBonusDeuxModeles;
    [SerializeField] private Joyaux[] _tJoyauxModeles;

    [Header("Paramètres")]
    [SerializeField] private int _nbBonusParSalle = 9;
    [SerializeField] private int _nbJoyauxParSalle = 9;

    [Header("Modèles d'Objets")]
    [SerializeField] private GameObject _clefModele;
    [SerializeField] private GameObject _porteModele;
    [SerializeField] private GameObject _activateurModele;
    [SerializeField] private GameObject _effectorModele;
    [SerializeField] private GameObject _perso;

    [Header("Données Personnage")]
    [SerializeField] private SOPerso _donneesPerso;

    [Header("Caméra")]
    [SerializeField] private Transform _transformVCameraConfiner;

    [Header("Ennemi")]
    [SerializeField] private GameObject _ennemiZachary;

    // SINGLETON
    static Niveau _instance;
    static public Niveau instance => _instance;

    // Autres membres et méthodes de notre script

    public GameObject ennemiZachary { get => _ennemiZachary; set => _ennemiZachary = value; }
    public int CompteurNiveau { get => _compteurNiveau; set => _compteurNiveau = value; }

    // LISTES
    List<Vector2Int> _lesPosLibres = new List<Vector2Int>();
    List<Vector2Int> _lesPosSurReperes = new List<Vector2Int>();

    // VARIABLES
    private int _compteurNiveau;


    void Awake()
    {
        // #synthese_ZACHARY
        // Variable qui compte le nombre de niveau afin de faire un niveau bonus à chaque 3 niveau
        _compteurNiveau = _donneesPerso.niveau;

        if (_compteurNiveau % 3 == 0) NiveauBonus();
        else
        {
            _clefModele.SetActive(true);
            _porteModele.SetActive(true);
        }

        if (_instance != null) { Destroy(gameObject); return; }
        _instance = this;

        // #TP4 ZACHARY
        // Si nous sommes au niveau 1, la taille de notre niveau sera de 3x3
        // Sinon, nous augmentons de +1 en x seulement
        if (_donneesPerso.niveau == 1)
        {
            _taille = new Vector2Int(3, 3);
        }
        // #synthese_ZACHARY
        // On limite une longueur de 5 salles pour le restant de la partie
        else if (_donneesPerso.niveau > 1 && _donneesPerso.niveau + 2 <= 5)
        {
            _taille = new Vector2Int(_donneesPerso.niveau + 2, 3);
        }
        else
        {
            _taille = new Vector2Int(5, 3);
        }



        Vector2Int tailleAvecUneBordure = Salle.taille - Vector2Int.one;

        tailleAvecUneBordure = CreerLesNiveaux(tailleAvecUneBordure);
        Vector2Int tailleTable = _taille * tailleAvecUneBordure;
        Vector2Int min = Vector2Int.zero - Salle.taille / 2;
        Vector2Int max = min + tailleTable;

        for (int y = min.y; y <= max.y; y++)
        {
            for (int x = min.x; x <= max.x; x++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (x == min.x || x == max.x || y == min.y || y == max.y) _tilemap.SetTile(pos, _tuileModele);
            }
        }

        // #synthese_ZACHARY
        // Réglage du confiner pour la caméra cinemachine
        // Calcul de la largeur du niveau (toutes les tuiles en largeur)
        int tailleX = (_taille.x * (Salle.taille.x - 1));
        int tailleY = (_taille.y * (Salle.taille.y - 1));

        _transformVCameraConfiner.localScale = new Vector3(tailleX, tailleY, 1);
        _transformVCameraConfiner.position = new Vector3(-Salle.taille.x / 2f, -Salle.taille.y / 2f, _transformVCameraConfiner.position.z);





        TrouverPosLibres();
        PlacerLesBonus();
        PlacerLesJoyaux();



        Debug.Log("Niveau: " + _donneesPerso.niveau);
    }

    /// <summary>
    /// On vient ici créer une nouvelle salle pour chaque coordonée grâce à la boucle en x et en y par notre Vector2Int.
    /// On fait un Instantiate de notre Salle à une position spécifique dans la grid afin que chacune d'entre elles soient une à côté de l'autre (x,y)
    /// C'est grâce à notre variable pos car elle calcule la position des cellules dans la grid (x,y) en tenant compte de notre variable tailleAvedUneBordure (-1)
    /// </summary>
    /// <param name="tailleAvecUneBordure"></param>
    /// <returns></returns>
    Vector2Int CreerLesNiveaux(Vector2Int tailleAvecUneBordure)
    {
        // #TP3 ZACHARY On positionne de facon aléatoire notre porte et notre clef
        Vector2Int placementClef, placementPorte, placementPerso;
        PositionnerAleatoireClefPortePerso(out placementClef, out placementPorte, out placementPerso);

        // #TP3 ZACHARY
        // On vérifie si la position de l'activateur correspond à celle de la clé ou de la porte
        // Si oui, on change sont emplacement de facon aléatoire jusqu'à temps que ca soit différent
        Vector2Int placementActivateur = new Vector2Int(Random.Range(0, _taille.x), Random.Range(0, _taille.y));// Générer la position aléatoire de l'activateur
        while (placementActivateur == placementClef || placementActivateur == placementPorte)
        {
            placementActivateur = new Vector2Int(Random.Range(0, _taille.x), Random.Range(0, _taille.y));
        }

        // Debug.Log("Emplacement " + placementClef);
        // On parcours la salle
        for (int x = 0; x < _taille.x; x++)
        {
            for (int y = 0; y < _taille.y; y++)
            {
                int choixSalle = Random.Range(0, _tSallesModeles.Length); // Permet de choisir une salle aléatoire
                Vector2 pos = new Vector2(tailleAvecUneBordure.x * x, tailleAvecUneBordure.y * y);
                Vector2Int placementSalle = new Vector2Int(x, y);
                Salle salle = Instantiate(_tSallesModeles[choixSalle], pos, Quaternion.identity, transform);
                //salle.gameObject.SetActive(false); // On enlève les copies des salles
                string nomModele = _tSallesModeles[choixSalle].gameObject.name; // On va chercher le nom de notre prefab
                // #synthese_ZACHARY
                // On vient ici placer notre salle dans notre tilemap selon la position de notre salle
                salle.name = $"Salle_{x}_{y}_{nomModele}";

                // #TP3 ZACHARY
                // On vérifie si la position actuelle de la salle correspond à la position de notre clef
                // On calcule la position de la clef sur le repère de la salle et on l'ajoute à la liste 
                PlacerClefPorteActivateurPerso(placementClef, placementPorte, placementActivateur, placementPerso, x, y, salle);
            }
        }

        return tailleAvecUneBordure;
    }

    /// <summary>
    /// #TP3 ZACHARY
    /// Méthode qui permet de choisir l'emplacement de la porte et de la clef de facon aléatoire
    /// Il y a 3 scénarios possible afin que la ceux-ci se retrouvent à chaque fois aux extrémités de chacun
    /// </summary>
    /// <param name="placementClef"></param>
    /// <param name="placementPorte"></param>
    void PositionnerAleatoireClefPortePerso(out Vector2Int placementClef, out Vector2Int placementPorte, out Vector2Int placementPerso)
    {
        // #TP3 ZACHARY 
        // On a une chance sur trois d'avoir un scénario, c'est-à-dire, clef gauche/porte droite ou l'inverse ainsi qu'en haut et en bas
        int chance = Random.Range(0, 3);
        if (chance == 0)
        {
            placementClef = new Vector2Int(_taille.x - 1, Random.Range(0, _taille.y)); // clé à droite
            placementPorte = new Vector2Int(0, Random.Range(0, _taille.y)); // porte à gauche

            // #synthese_ZACHARY
            // On vient ici placer le personnage de facon aléatoire dans une salle aléatoire sans que ce soit la clé ou la porte
            // Avant, il pouvait se retrouver sur la clé ou la porte
            do
            {
                placementPerso = new Vector2Int(Random.Range(0, _taille.x), Random.Range(0, _taille.y));
            } while (placementPerso == placementClef || placementPerso == placementPorte);
        }
        else if (chance == 1)
        {
            placementClef = new Vector2Int(Random.Range(0, _taille.x), _taille.y - 1); // clé en haut
            placementPorte = new Vector2Int(Random.Range(0, _taille.x), 0); // porte en bas

            // #synthese_ZACHARY
            // On vient ici placer le personnage de facon aléatoire dans une salle aléatoire sans que ce soit la clé ou la porte
            // Avant, il pouvait se retrouver sur la clé ou la porte
            do
            {
                placementPerso = new Vector2Int(Random.Range(0, _taille.x), Random.Range(0, _taille.y));
            } while (placementPerso == placementClef || placementPerso == placementPorte);
        }
        else
        {
            placementClef = new Vector2Int(0, Random.Range(0, _taille.y)); // clé à gauche
            placementPorte = new Vector2Int(_taille.x - 1, Random.Range(0, _taille.y)); // porte à droite

            // #synthese_ZACHARY
            // On vient ici placer le personnage de facon aléatoire dans une salle aléatoire sans que ce soit la clé ou la porte
            // Avant, il pouvait se retrouver sur la clé ou la porte
            do
            {
                placementPerso = new Vector2Int(Random.Range(0, _taille.x), Random.Range(0, _taille.y));
            } while (placementPerso == placementClef || placementPerso == placementPorte);
        }
    }

    /// <summary>
    /// #TP3 ZACHARY
    /// Méthode qui place notre porte, clef et activateur selon les positions libres dans notre TM et les place sur notre repère
    /// La clef et la porte est défénit de facon aléatoire grâce à notre fonction PositionnerAleatoireClefPorte
    /// Alors que notre activateur est definit grâce à notre boucle while dans la fonction CreerLesNiveaux pour que celui-ci n'empile pas sur un autre objet
    /// </summary>
    /// <param name="placementClef"></param>
    /// <param name="placementPorte"></param>
    /// <param name="placementActivateur"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="salle"></param>
    private void PlacerClefPorteActivateurPerso(Vector2Int placementClef, Vector2Int placementPorte, Vector2Int placementActivateur, Vector2Int placementPerso, int x, int y, Salle salle)
    {
        // #TP3 ZACHARY
        if (placementClef == new Vector2Int(x, y))
        {
            Vector2Int decalage = Vector2Int.CeilToInt(_tilemap.transform.position);
            Vector2Int posRepere = salle.PlacerSurRepere(_clefModele) - decalage;
            _lesPosSurReperes.Add(posRepere);
        }
        // #TP3 ZACHARY
        if (placementPorte == new Vector2Int(x, y))
        {
            Vector2Int decalage = Vector2Int.CeilToInt(_tilemap.transform.position);
            Vector2Int posRepere = salle.PlacerSurRepere(_porteModele) - decalage;
            _lesPosSurReperes.Add(posRepere);
        }
        // #TP3 ZACHARY
        if (placementActivateur == new Vector2Int(x, y))
        {
            Vector2Int decalage = Vector2Int.CeilToInt(_tilemap.transform.position);
            Vector2Int posRepere = salle.PlacerSurRepere(_activateurModele) - decalage;
            _lesPosSurReperes.Add(posRepere);
        }

        // #synthese_ZACHARY
        // On place notre perso dans la salle grâce à un repère
        // On ajoute ensuite la position de celui-ci dans notre liste
        // Avant il apparraissait n'importe où dans une salle et parfois dans une tuile.   	    
        if (placementPerso == new Vector2Int(x, y))
        {
            Vector2Int decalage = Vector2Int.CeilToInt(_tilemap.transform.position);
            Vector2Int posRepere = salle.TransformSurRepere(_perso) - decalage;
            _lesPosSurReperes.Add(posRepere);
        }
    }

    /// <summary>
    /// #TP3 ZACHARY
    /// Fonction qui permet de trouver toute les tuiles libres à l'intérieur de ma TileMap grâce au Bounds
    /// On ajoute ensuite les coordonnées à l'intérieur de notre liste
    /// </summary>
    public void TrouverPosLibres()
    {
        BoundsInt bornes = _tilemap.cellBounds;
        for (int x = bornes.xMin; x < bornes.xMax; x++)
        {
            for (int y = bornes.yMin; y < bornes.yMax; y++)
            {
                Vector2Int posTuile = new Vector2Int(x, y);
                TileBase tuile = _tilemap.GetTile((Vector3Int)posTuile);
                if (tuile == null) _lesPosLibres.Add(posTuile);
            }
        }
        // Pour chaque position, on enleve la position de la liste
        // On ne veut pas 2 objets sur un seul repere
        foreach (Vector2Int pos in _lesPosSurReperes)
        {
            _lesPosLibres.Remove(pos);
        }
        // Debug.Log(string.Join(",", _lesPosLibres));
    }

    /// <summary>
    /// #TP3 ZACHARY 
    /// Fonction qui permet d'ajouter des bonus de facon aléatoire à travers les différentes salles
    /// On ajoute les bonus de facon aléatoire 
    /// On va aussi chercher la position afin de l'instancier parmis les positions libres dans notre liste
    /// </summary>
    void PlacerLesBonus()
    {
        Transform contenant = new GameObject("Bonus").transform; // On vient glisser nos bonus à l'intérieur de ce GameObject vide
        contenant.parent = transform; // On devient l'enfant de niveau

        int nbBonus = _nbBonusParSalle;
        for (int i = 0; i < nbBonus; i++)
        {
            int indexBonus = Random.Range(0, _tBonusModeles.Length);
            Bonus bonusModele = _tBonusModeles[indexBonus]; // On a notre bonus

            int indexBonusDeux = Random.Range(0, _tBonusDeuxModeles.Length);
            Bonus2 bonusDeuxModele = _tBonusDeuxModeles[indexBonusDeux]; // On a notre bonus 2

            // On vient chercher la position libre dans notre salle pour les deux bonus sous un vector 3
            Vector2Int pos = ObtenirUnePoseLibre();
            Vector2Int pos2 = ObtenirUnePoseLibre();

            // On vient créer nos deux bonus aléatoirement sur des positions innocupés grâce a pos et pos2
            // On ajoute ces positions dans notre tilemap grace a tileAnchor afin de l'ajouter dans notre grille
            Vector3 pos3 = (Vector3)(Vector2)pos + _tilemap.transform.position + _tilemap.tileAnchor;
            Instantiate(bonusModele, pos3, Quaternion.identity, contenant);
            Vector3 pos3Deux = (Vector3)(Vector2)pos2 + _tilemap.transform.position + _tilemap.tileAnchor;
            Instantiate(bonusDeuxModele, pos3Deux, Quaternion.identity, contenant);

            if (_lesPosLibres.Count == 0) { break; } // On casse la boucle lorsqu'on arrive à la fin de la liste
        }
    }

    /// <summary>
    ///#TP3 ZACHARY
    /// Fonction qui permet d'obtenir une pose libre dans notre salle 
    /// Pour ensuite enlever les coordonnées de notre liste afin de ne pas répéter 2 objets à la même place
    /// </summary>
    /// <returns></returns>
    Vector2Int ObtenirUnePoseLibre()
    {
        int indexPosLibre = Random.Range(0, _lesPosLibres.Count);
        Vector2Int pos = _lesPosLibres[indexPosLibre];
        _lesPosLibres.RemoveAt(indexPosLibre); // On enlève afin de pas répéter la même position 
        return pos;
    }


    /// <summary>
    ///#TP3 Albert
    /// Fonction qui permet d'ajouter des Joyaux de facon aléatoire à travers les différentes salles
    /// On ajoute les joyaux de facon aléatoire selon la grandeur de notre tableau
    /// On va aussi chercher la position afin de l'instancier parmis les positions libres dans notre liste
    /// </summary>
    void PlacerLesJoyaux()
    {
        Transform contenant = new GameObject("Joyaux").transform; // On vient glisser nos Joyaux à l'intérieur de ce GameObject vide
        contenant.parent = transform; // On devient l'enfant de niveau

        int nbJoyaux = _nbJoyauxParSalle;

        // #synthese_ZACHARY
        // Si nous sommes dans notre niveau bonus, on ajoute 30 joyaux de plus
        // Sinon, on ajoute le nombre de joyaux par salle
        if (_compteurNiveau % 3 == 0) nbJoyaux = _nbJoyauxParSalle * _compteurNiveau + 50;
        else { nbJoyaux = _nbJoyauxParSalle; }

        for (int i = 0; i < nbJoyaux; i++)
        {
            int indexJoyaux = Random.Range(0, _tJoyauxModeles.Length);
            Joyaux JoyauxModele = _tJoyauxModeles[indexJoyaux]; // On a notre Joyaux

            Vector2Int pos = ObtenirUnePoseLibre(); // Pour le Joyaux 1
            Vector2Int pos2 = ObtenirUnePoseLibre(); // Pour le Joyaux 2

            Vector2Int posJoyaux = ObtenirUnePoseLibre(); // Pour le Joyaux

            Vector3 pos3 = (Vector3)(Vector2)pos + _tilemap.transform.position + _tilemap.tileAnchor; // Pour le Joyaux 1
            Instantiate(JoyauxModele, pos3, Quaternion.identity, contenant); // Pour le Joyaux 1

            if (_lesPosLibres.Count == 0) { break; } // On casse la boucle lorsqu'on arrive à la fin de la liste
        }
    }

    /// <summary>
    /// #synthese_ZACHARY
    /// Fonction qui permet de compter mes niveaux et de faire un niveau bonus à chaque 3 niveaux (3,6,9,12,15,18,21,24,27,30....)
    /// On vient chercher le niveau du joueur et on le compare à notre compteur de niveau
    /// Ce niveau est une course contre la montre
    /// Il n'y a pas de clef ni de porte
    /// Il y a moins de temps dans ce niveau bonus
    /// </summary>
    void NiveauBonus()
    {
        _compteurNiveau = _donneesPerso.niveau;
        _nbBonusParSalle = 1; // On réduit de facon considérable le nombre de bonus
        _clefModele.SetActive(false); // On enlève la clef
        _porteModele.SetActive(false); // On enlève la porte

    }
}
