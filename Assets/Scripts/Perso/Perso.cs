using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Auteur : Zachary Chandonnet
/// Classe qui controle les déplacements et saut du personnage 
/// Auteurs du code: Zachary Chandonnet et Albert Jannard (TP3) #Synthese_ALBERT
/// Auteurs des commentaires: Zachary Chandonnet et Albert Jannard (Tp3) #Synthese_ALBERT
/// </summary>
public class Perso : BasePerso
{
    public Shuriken shuriken;

    [Header("Audio")]
    [SerializeField] AudioClip _sonDeSaut;
    [SerializeField] AudioClip _sonDAtterrissage;
    [SerializeField] AudioClip sonDegas; // #Synthese_ALBERT Jannard
    [SerializeField] AudioClip lancerShuriken; // #Synthese_ALBERT Jannard
    [SerializeField] AudioClip potion; // #Synthese_ALBERT Jannard

    [Header("Paramètres du Personnage")]
    [SerializeField] float _vitessePersonnage = 9;
    public float vitessePersonnage { get => _vitessePersonnage; set => _vitessePersonnage = value; }
    [SerializeField] float _forceSaut;
    public float forceSaut { get => _forceSaut; set => _forceSaut = value; }

    [SerializeField] int _nbFramesMax = 6;

    [Header("Clignotement")]
    [SerializeField] float _clignotementDuree = 0.1f;
    [SerializeField] int _nbClignotement = 5;

    [Header("Force")]

    Vector3 _scaleDepart;

    [Header("Shuriken")]
    [SerializeField] GameObject _shurikenPrefab;
    [SerializeField] Transform _shurikenReperePoint;
    [SerializeField] float shurikenVitesse = 12f;
    [SerializeField] float tempsRechargeShuriken = 0.7f;

    bool _estEnChute = false;

    [Header("Composants")]
    Rigidbody2D _rb;
    SpriteRenderer _sr;
    Animator _anim;
    Collider2D persoCollider;


    [Header("Variables")]
    int _nbFramesRestants = 0;

    float _axeHorizontal;

    bool _persoVeutSauter = false;
    bool _contactEnnemi = false;

    bool _potionActive = false;
    public bool potionActive { get => _potionActive; set => _potionActive = value; }
    bool _peuxSortir = false;
    public bool peuxSortir { get => _peuxSortir; set => _peuxSortir = value; }
    float dernierTempsLancementShuriken = 0f;

    [SerializeField] private SOPerso _donnees;
    bool _aEffectueDoubleSaut = false;


    private int nombreSauts = 0;

    void Start()
    {
        // J'attribue mes components à chaque variable
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
        persoCollider = GetComponent<Collider2D>();
        _scaleDepart = transform.localScale;

        

        GestAudio.instance.SonDeSaut = _sonDeSaut;
        GestAudio.instance.SonDAtterrissage = _sonDAtterrissage;
    }


    void Update()
    {
        GestionnaireDesAnimation();

        // Permet de controler les actions du personnage
        _axeHorizontal = Input.GetAxis("Horizontal");
        _persoVeutSauter = Input.GetButton("Jump");

        // Pour la fonctionnalité du double saut 
        if (Input.GetButtonDown("Jump"))
        {
            nombreSauts++; // Incrémente le nombre de sauts lorsque le personnage saute
            if (nombreSauts == 2 && _donnees.nbPotionsDoubleSaut > 0)
            {
                DoubleSauter();
            }
        }

        // Permet d'inverser le sprite en X s'il se déplace vers la gauche ou vers la droite
        if (_axeHorizontal < 0)
        {
            _sr.flipX = true;

        }
        else if (_axeHorizontal > 0) _sr.flipX = false;

        GestionSonChute();

        if (_contactEnnemi) _forceSaut = 10f;
        else _forceSaut = 320f;

        if (Input.GetMouseButtonDown(0))
        {
            LancerShuriken();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            UtiliserPotion();
        }

        
        
    }
    
    
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        CourirPerso();

        if (_estAuSol)
        {
            if (_persoVeutSauter)
            {
                Sauter();

            }
            else
            {
                _nbFramesRestants = _nbFramesMax;
            }
        }
        else
        {
            bool peutSauterPlus = (_nbFramesRestants > 0);
            if (_persoVeutSauter && peutSauterPlus && !_aEffectueDoubleSaut) // Vérifie si le personnage peut effectuer un double saut
            {
                Sauter();
                _aEffectueDoubleSaut = true;
            }
            else
            {
                _nbFramesRestants = 0;
            }
        }
    }

    /// <summary>
    /// Fonction qui gère le déplacement horizontal du personnage. 
    /// Permet de nous déplacer vers la gauche/droite selon le getAxis et la vitesse dans un vector 2
    /// </summary>
    void CourirPerso()
    {
        // Permet de nous déplacer vers la gauche/droite selon le getAxis et la vitesse dans un vector 2
        _rb.velocity = new Vector2(_axeHorizontal * _vitessePersonnage, _rb.velocity.y);
    }

    /// <summary>
    /// #Synthese_ALBERT
    /// Fonction qui permet les animations du personnage. 
    /// Permet de changer de blend tree selon si le personnage est au sol ou non
    /// Permet de changer l'animation du personnage selon sa vitesse en X
    /// Permet de changer l'animation du personnage selon sa vitesse en Y
    /// Utilise la vélocité du personnage pour changer l'animation
    /// </summary>
    void GestionnaireDesAnimation()
    {
        // Permets de changer de blend tree selon si le personnage est au sol ou non 
        if (_estAuSol) _anim.SetBool("estAuSol", true);
        else _anim.SetBool("estAuSol", false);

        // Permet de changer l'animation du personnage selon sa vitesse en X 
        if (_rb.velocity.x > 0.1f) _anim.SetFloat("vitesseX", 1);
        else if (_rb.velocity.x < -0.1f) _anim.SetFloat("vitesseX", -1);
        else _anim.SetFloat("vitesseX", 0);

        // Permet de changer l'animation du personnage selon sa vitesse en Y 
        if (_rb.velocity.y > 1f) _anim.SetFloat("vitesseY", 1);
        else if (_rb.velocity.y < -1f) _anim.SetFloat("vitesseY", -1);
        else _anim.SetFloat("vitesseY", 0);

    }

    /// <summary>
    /// Fonction qui permet au personnage de sauter lorsque le joueur appuie sur la touche espace.
    /// Elle agit selon la la force du saut ainsi que le nombre de frames maximum autorisé.
    /// Plus que le joueur maintient la touche espace, puisque le personnage sautera plus haut.
    /// </summary>

    void Sauter()
    {
        float fractionForce = (float)_nbFramesRestants / _nbFramesMax;
        float puissance = _forceSaut * fractionForce;
        // float puissance = _forceSaut;
        _rb.AddForce(Vector2.up * puissance);

        GestAudio.instance.JouerEffetSonore(_sonDeSaut); // TP#4 Albert Jannard

        _nbFramesRestants--;
        if (_nbFramesRestants < 0) _nbFramesRestants = 0;

    }

    /// <summary>
    /// Fonction qui gere le son d'atterisage du personnage
    /// </summary>
    void GestionSonChute() // TP#4 Albert Jannard
    {
        if (!_estAuSol)
        {
            _estEnChute = true;
        }
        else if (_estAuSol && _estEnChute)
        {
            GestAudio.instance.JouerEffetSonore(_sonDAtterrissage);
            _estEnChute = false;
            nombreSauts = 0;
        }
    }

    /// <summary>
    /// Fonction qui permet de faire augmenter la somme d'argent du jouer
    /// Auteur: Albert Jannard
    /// </summary>
    public void AugmenterArgent(int nbValeur)
    {
        _donnees.argent += nbValeur; // Ajoute au montent d'argent la valeur du joyaux récupérer 
    }

    /// <summary>
    /// Callback sent to all game objects before the application is quit.
    /// </summary>
    void OnApplicationQuit()
    {
        _donnees.Initialiser();
    }

    /// <summary>
    /// #synthèse_ZACHARY
    /// Fonction qui permet de faire clignoter le personnage lorsqu'il entre en collision avec un ennemi (lui tombe sur la tete)
    /// Elle est appelée dans la fonction OnCollisionEnter2D
    /// </summary>
    /// <returns></returns>
    private IEnumerator ClignoterPersonnage()
    {
        for (int i = 0; i < _nbClignotement; i++)
        {
            // On change la couleur du sprite du personnage pour simuler le clignotement
            _sr.color = new Color(1f, 1f, 1f, 0f); // transparent
            yield return new WaitForSeconds(_clignotementDuree / 2);
            _sr.color = new Color(1f, 1f, 1f, 1f); // opaque
            yield return new WaitForSeconds(_clignotementDuree / 2);
        }
    }


    /// <summary>
    /// #synthèse_ZACHARY
    /// Fonction qui permet de détecter la collision entre le personnage et un ennemi
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si le personnage entre en collision avec un ennemi, on déclenche le clignotement
        if (other.CompareTag("Ennemi"))
        {
            StartCoroutine(ClignoterPersonnage());

        }

        // Si le personnage entre en collision avec un projectile, on déclenche le rétrécissement
        if (other.CompareTag("Projectile"))
        {
            StartCoroutine(AttribuPersoProjectile());
        }
    }


    /// <summary>
    /// #synthèse_ZACHARY
    /// Fonction qui permet de détecter la collision entre le personnage et un ennemi
    /// </summary>
    /// <param name="other">The Collision2D data associated with this collision.</param>
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ennemi"))
        {
            StartCoroutine(AttribuPerso());
            GestAudio.instance.JouerEffetSonore(sonDegas);
        }
    }

    /// <summary>
    /// #synthèse_ZACHARY
    /// Fonction qui permet de réduire la taille du personnage lorsqu'il entre en collision avec un ennemi pendant 5 secondes
    /// Elle est appelée dans la fonction OnCollisionEnter2D et est gérée par dans le update (variable contactEnnemi)
    /// </summary>
    /// <returns></returns>
    IEnumerator AttribuPerso()
    {
        // On change le scale du personnage pour le faire rétrécir
        Vector3 scaleActuel = transform.localScale;
        transform.localScale = new Vector3(scaleActuel.x / 2, scaleActuel.y / 2, scaleActuel.z / 2);
        _contactEnnemi = true;

        // Attente de 3 secondes
        yield return new WaitForSeconds(3f);

        // On remet le scale du personnage à sa valeur initiale
        transform.localScale = _scaleDepart;
        _contactEnnemi = false;
    }

    /// <summary>
    /// #synthèse_ZACHARY
    /// Fonction qui permet de faire clignoter le personnage lorsqu'il entre en collision avec un projectile
    /// Le personnage clignote en rouge pendant 2 secondes
    /// </summary>
    /// <returns></returns>
    IEnumerator AttribuPersoProjectile()
    {
        Color couleurOriginale = _sr.color; // Sauvegarde de la couleur originale du sprite

        for (int i = 0; i < _nbClignotement; i++)
        {
            // On change la couleur du sprite du personnage pour simuler le clignotement en rouge
            _sr.color = Color.red; // Couleur rouge
            yield return new WaitForSeconds(_clignotementDuree / 2);
            _sr.color = couleurOriginale; // Restauration de la couleur originale
            yield return new WaitForSeconds(_clignotementDuree / 2);
        }

        // Restauration de la couleur originale du sprite
        _sr.color = couleurOriginale;
    }

    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    // #Synthèse_ALBERT 
    // Partie de code qui gère le lancement de shuriken utilisation des potions de soins
    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

    /// <summary> #Synthèse_ALBERT
    /// Fonction qui permet de lancer un shuriken
    /// </summary>
    public void LancerShuriken()
    {

        // Vérifie si suffisamment de temps s'est écoulé depuis le dernier lancement de shuriken
        if (Time.time - dernierTempsLancementShuriken >= tempsRechargeShuriken && _donnees.nbShuriken > 0)
        {
            // Calcule la position de la souris dans le monde 2D
            Vector3 positionSouris = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            positionSouris.z = 0f; // Fixe la valeur de la profondeur à zéro

            // Calcule la direction entre la position de lancement du shuriken et la position de la souris
            Vector3 directionShuriken = positionSouris - _shurikenReperePoint.position;
            directionShuriken.Normalize();

            // Instancie le shuriken au point de lancement
            GameObject shuriken = Instantiate(_shurikenPrefab, _shurikenReperePoint.position, Quaternion.identity);

            // Inverse l'échelle du shuriken s'il va vers la gauche
            if (directionShuriken.x < 0)
            {
                Vector3 echelle = shuriken.transform.localScale;
                echelle.x = -echelle.x;
                shuriken.transform.localScale = echelle;
            }

            // Applique une force au shuriken dans la direction de la souris
            Rigidbody2D rigidbodyShuriken = shuriken.GetComponent<Rigidbody2D>();
            rigidbodyShuriken.AddForce(directionShuriken * shurikenVitesse, ForceMode2D.Impulse);

            // Enregistre le temps du dernier lancement de shuriken
            dernierTempsLancementShuriken = Time.time;
            _donnees.nbShuriken--; // Enlève un shuriken à la quantité de shurikens restants

            GestAudio.instance.JouerEffetSonore(lancerShuriken);
        }


    }

    /// <summary> #Synthèse_ALBERT
    /// Fonction qui gère la potion de soins
    /// </summary>
    public void UtiliserPotion()
    {
        int viePotions = 1; // Quantité de vie que la potion redonne
        // Vérifie si le joueur a des potions de soins
        if (_donnees.nbPotions > 0)
        {
            // Augmente la vie du joueur
            _donnees.vie += viePotions;


            // Enlève une potion de soins à la quantité de potions restantes
            _donnees.nbPotions--;
            GestAudio.instance.JouerEffetSonore(potion);
        }
    }

    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    // #Synthèse_ALBERT 
    // Partie de code qui permet le double saut
    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

    /// <summary>
    /// #Synthèse_ALBERT
    /// Fonction qui gère le double saut du personnage
    /// </summary>
    void DoubleSauter()
    {
        // Appliquer une force verticale au personnage pour le faire effectuer un double saut
        _rb.AddForce(new Vector2(0f, 15f), ForceMode2D.Impulse);
        _aEffectueDoubleSaut = true;

        _anim.SetBool("saut", true);
        GestAudio.instance.JouerEffetSonore(_sonDeSaut);
        _nbFramesRestants = _nbFramesMax;

        _donnees.nbPotionsDoubleSaut--; // Enlève une potion de double saut à la quantité de potions restantes
    }

}