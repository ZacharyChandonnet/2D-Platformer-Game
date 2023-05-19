using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesController : MonoBehaviour
{
    [Header("Particules")]
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private bool vasADroite = true;
    private float _particleOffset = 0.4f;

    private float _axeHorizontal;
    private void Start()
    {
        particles = GetComponentInChildren<ParticleSystem>(); // Obtenez le système de particules enfant du personnage
    }

    private void Update()
    {
        _axeHorizontal = Input.GetAxis("Horizontal");
        ChangerLorientationParticule();
    }

    private void ChangerLorientationParticule()
    {
        if (_axeHorizontal < 0 && vasADroite) // Si le personnage se déplace vers la gauche et regarde vers la droite
        {
            particles.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0)); // Faites pivoter le système de particules pour qu'il suive la direction gauche

            vasADroite = false; // Mettez à jour la direction du personnage
            _particleOffset = 0.3f; // Inversez la valeur de l'offset des particules en X


        }
        else if (_axeHorizontal > 0 && !vasADroite) // Si le personnage se déplace vers la droite et regarde vers la gauche
        {
            particles.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0)); // Faites pivoter le système de particules pour qu'il suive la direction droite

            vasADroite = true; // Mettez à jour la direction du personnage
            _particleOffset = -0.3f; // Inversez la valeur de l'offset des particules en X
        }

        // Définir la position des particules en fonction de la position du personnage et de l'offset des particules
        Vector3 newPosition = new Vector3(transform.position.x + _particleOffset, particles.transform.position.y, particles.transform.position.z);
        particles.transform.position = newPosition;
    }
}
