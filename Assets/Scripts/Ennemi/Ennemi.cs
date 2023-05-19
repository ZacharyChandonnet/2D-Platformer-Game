using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Auteur : Zachary Chandonnet
/// #synthese_ZACHARY
/// Classe qui gère les les mouvements des ennemis
/// Elle utilise un rigidbody pour déplacer l'ennemi vers les destinations & une liste de destinations pour déterminer les destinations de l'ennemi
/// Elle utilise un délai pour le premier départ, un délai pour les départs suivants, une tolérance pour la distance entre l'ennemi et la destination, 
/// une durée de placement pour le déplacement de l'ennemi.
/// </summary>
public class Ennemi : MonoBehaviour
{
    [Header("Destinations")]
    [SerializeField] private Transform[] _destinations;
    [SerializeField] private float _delaiPremierDepart = 0.5f;
    [SerializeField] private float _delaiDepartsSuivants = 0.5f;
    [SerializeField] private float _toleranceDest = 0.6f;
    [SerializeField] private float _dureePlacement = 3.5f;

    [Header("Ennemi")]
    [SerializeField] private GameObject _ennemi;
    [SerializeField] private SOPerso _donneesPerso;

    [Header("Sons")]
    [SerializeField] private AudioClip degatsSon;
    [SerializeField] private AudioClip mortSon;

    private Vector2 _posIni;
    private float _tempsActuel;
    private int _iDest = 0;

    private bool _estMort = false;
    private float _santeMaximale = 20f; // Santé maximale de l'ennemi // #Synthèse_ALBERT
    private float _santeActuelle; // Santé actuelle de l'ennemi // #Synthèse_ALBERT

    private int _dommageEnnemi = 1; // Dommage que l'ennemi inflige au joueur 

    private Rigidbody2D _rb;
    private Animator _anim;


    void Awake()
    {
        // Initialiser le rigidbody et le mettre à la première destination de la liste 
        _rb = GetComponent<Rigidbody2D>();
        _rb.MovePosition(_destinations[0].position);
        _anim = GetComponent<Animator>();
        // _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        StartCoroutine(CoroutineGererTrajet());
        _santeActuelle = _santeMaximale; // #Synthèse_ALBERT
        SanteEnnemiBonus();
    }


/// <summary>
    /// #synthese_ZACHARY
    /// Fonction qui double la santé de l'ennemi à chaque 3 niveaux (mon niveau bonus)
    /// </summary>
    private void SanteEnnemiBonus()
    {
        // #synthese_ZACHARY
        // Si le niveau est un multiple de 3, on double la santé de l'ennemi
        // Sinon on garde la santé normale
        if (_donneesPerso.niveau % 3 == 0) _santeMaximale = _santeMaximale * 2;
        _santeActuelle = _santeMaximale;

    }
    /// <summary>
    /// #synthese_ZACHARY
    /// Coroutine qui gère le trajet de l'ennemi
    /// Elle attend un certain temps avant de démarrer et ensuite elle boucle à l'infini en attendant un certain temps entre chaque déplacement vers une destination différente 
    /// de la liste de destinations (Nos repères)
    /// Elle utilise la fonction BougerAvecLerp pour déplacer l'ennemi vers la destination
    /// </summary>
    /// <returns></returns>
    IEnumerator CoroutineGererTrajet()
    {
        //attendre avant de démarrer
        yield return new WaitForSeconds(_delaiPremierDepart);
        // On boucle à l'infini afin de faire bouger l'ennemi vers les destinations
        while (!_estMort)
        {
            // Obtenir la prochaine destination
            Vector2 posDest = ObtenirPosProchaineDestination();
            _posIni = transform.position;
            _tempsActuel = 0f;
            // Bouger vers la destination tant que la distance est plus grande que la tolérance 
            while (Vector2.Distance(transform.position, posDest) > _toleranceDest) //tant que la distance est plus grande que la tolérance
            {
                BougerAvecLerp(posDest); //ajouter de la force vers la destination
                yield return new WaitForFixedUpdate(); //attendre la prochaine frame (physique!)
            }

            yield return new WaitForSeconds(_delaiDepartsSuivants);
            //attendre avant de démarrer
        }

    }

    /// <summary>
    /// #synthese_ZACHARY
    /// Fonction qui déplace l'ennemi vers une destination avec une force lerp
    /// Elle utilise un ratio pour déterminer la position de l'ennemi entre sa position initiale et la destination
    /// Elle utilise la fonction SmoothStep pour avoir un mouvement plus fluide & la fonction MovePosition du rigidbody pour déplacer l'ennemi
    /// Elle utilise le temps actuel et la durée de placement pour déterminer le ratio & la position initiale et la destination pour déterminer le ratio
    /// </summary>
    /// <param name="posDest"></param>
    void BougerAvecLerp(Vector2 posDest)
    {
        _tempsActuel += Time.fixedDeltaTime;
        float ratio = _tempsActuel / _dureePlacement;
        ratio = Mathf.SmoothStep(0f, 1f, ratio);
        Vector2 nouvPos = Vector2.Lerp(_posIni, posDest, ratio);

        // Calculer la direction de déplacement
        Vector2 direction = nouvPos - _rb.position;

        // Faire tourner l'ennemi selon la direction
        if (direction.x > 0f)
        {
            // Tourner vers la droite
            _ennemi.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (direction.x < 0f)
        {
            // Tourner vers la gauche
            _ennemi.transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        _rb.MovePosition(nouvPos);
    }

    /// <summary>
    /// #synthese_ZACHARY
    /// Fonction qui retourne la prochaine destination de la liste de destinations
    /// Elle incrémente le iDest pour aller chercher la prochaine destination pour ensuite la retourner 
    /// Si le iDest est plus grand que la longueur de la liste de destinations, elle le remet à 0
    /// Et on retourne la position de la destination
    /// </summary>
    /// <returns></returns>
    Vector2 ObtenirPosProchaineDestination()
    {
        _iDest++;
        if (_iDest >= _destinations.Length) _iDest = 0;
        Vector2 pos = _destinations[_iDest].position;
        return pos; //temporaire
    }


    /// <summary>
    /// #synthese_ZACHARY
    /// Fonction qui gère les collisions avec l'ennemi lorsqu'il entre en collision avec un notre personnage
    /// collider (2D physics only).
    /// </summary>
    /// <param name="other">The Collision2D data associated with this collision.</param>
    void OnCollisionEnter2D(Collision2D other)
    {
        // Si le joueur entre en collision avec le perso, on le détruit
        if (other.gameObject.CompareTag("Player"))
        {
            // #synthese_ZACHARY
            // On perd deux fois plus de vie si nous sommes dans un niveau bonus
            // Sinon on perd la vie normale
            if (_donneesPerso.niveau % 3 == 0) _donneesPerso.vie -= _dommageEnnemi * 2;
            else _donneesPerso.vie -= _dommageEnnemi;

        }
    }
    /// <summary>
    /// #Synthèse_ALBERT
    /// Inflige des dégâts à l'ennemi.
    /// </summary>
    /// <param name="montant">Le montant de dégâts à infliger.</param>
    public void PrendreDegats(float montant)
    {
        _santeActuelle -= montant;
        // Debug.Log("Santé actuelle : " + _santeActuelle);
        // Vérifie si l'ennemi est mort
        if (_santeActuelle <= 0f)
        {
            Mourir();
        }
        GestAudio.instance.JouerEffetSonore(degatsSon); // #synthese_ALBERT
    }

    /// <summary>
    /// #synthese_ZACHARY
    /// Fonction qui gère la mort de l'ennemi
    /// On donne 10 joyaux au joueur lorsqu'il tue un ennemi
    /// </summary>
    void Mourir()
    {
        _estMort = true; // L'ennemi est mort
        // Ajoute ici l'effet de mort de l'ennemi, comme une explosion ou une animation
        GestAudio.instance.JouerEffetSonore(mortSon); // #synthese_ALBERT 
        _anim.SetBool("mort", true);
        StartCoroutine(DelayedDestroy());
        _donneesPerso.argent += 10;
    }

    /// <summary>
    /// #synthese_ZACHARY
    /// Coroutine qui détruit le gameObject après un certain temps
    /// Afin de laisser le temps à l'animation de mort de se jouer
    /// </summary>
    /// <returns></returns>
    IEnumerator DelayedDestroy()
    {
        // Attends pendant un certain temps avant de détruire le gameObject
        yield return new WaitForSeconds(0.5f); // Remplacez 2f par la durée de votre animation de mort

        // Détruit le gameObject
        Destroy(gameObject);
    }
}
