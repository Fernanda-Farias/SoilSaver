using UnityEngine;
using UnityEngine.AI;
using TMPro; 
using UnityEngine.SceneManagement; 

public class Movimento : MonoBehaviour
{
    private NavMeshAgent agente;
    
    [Header("Configurações de Energia/Tempo")]
    public float energia = 100f; 
    public float velocidadeDoTempo = 3f; 
    public TMP_Text textoEnergia; 
    public GameObject telaMorte; 

    [Header("Configurações de Vitória")]
    public TMP_Text textoPontos; 
    public GameObject telaVitoria; 
    public int metaNutrientes = 5; 
    private int nutrientesColetados = 0; 

    [Header("Configurações de Início")]
    public GameObject telaInicial; 
    private bool jogoIniciado = false; 
    private static bool menuJaFoiVisto = false; 
    private bool jogoFinalizado = false; 

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        
        if (telaMorte != null) telaMorte.SetActive(false); 
        if (telaVitoria != null) telaVitoria.SetActive(false);
        
        if (menuJaFoiVisto)
        {
            if (telaInicial != null) telaInicial.SetActive(false);
            jogoIniciado = true;
        }
        else
        {
            if (telaInicial != null) telaInicial.SetActive(true);
        }
        
        AtualizarInterface();
    }

    void Update()
    {
        if (!jogoIniciado || jogoFinalizado) return; 

        energia -= velocidadeDoTempo * Time.deltaTime;
        
        if (textoEnergia != null)
        {
            textoEnergia.text = "ENERGIA: " + Mathf.Round(energia).ToString();
        }

        if (energia <= 0)
        {
            Falhar();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray raio = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit acerto;

            
            if (Physics.Raycast(raio, out acerto, Mathf.Infinity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
            {
                if (acerto.collider.CompareTag("Nutriente"))
                {
                    energia += 5f; // Bônus por mirar e clicar no alvo certo
                    if (energia > 100f) energia = 100f; 
                    agente.SetDestination(acerto.point);
                }
                else
                {
                    energia -= 5f; // Penalidade por errar o clique e acertar o chão
                    agente.SetDestination(acerto.point);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider outro)
    {
        if (!jogoIniciado || jogoFinalizado) return;

        if (outro.CompareTag("Nutriente"))
        {
            energia += 15f; // Dá um bônus maior de energia ao de fato "comer" o nutriente
            if (energia > 100f) energia = 100f; 

            nutrientesColetados++; 
            AtualizarInterface();
            
            Destroy(outro.gameObject); 

            if (nutrientesColetados >= metaNutrientes)
            {
                Vencer();
            }
        }
    }

    void UpdateInterface()
    {
        AtualizarInterface();
    }

    void AtualizarInterface()
    {
        if (textoPontos != null)
        {
            textoPontos.text = "NUTRIENTES: " + nutrientesColetados + " / " + metaNutrientes;
        }
    }

    private void Falhar()
    {
        jogoFinalizado = true;
        agente.isStopped = true; 
        if (telaMorte != null) telaMorte.SetActive(true); 
    }

    private void Vencer()
    {
        jogoFinalizado = true;
        agente.isStopped = true; 
        if (telaVitoria != null) telaVitoria.SetActive(true); 
    }

    public void ReiniciarJogo()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ComecarJogo()
    {
        jogoIniciado = true;
        menuJaFoiVisto = true; 
        if (telaInicial != null) telaInicial.SetActive(false); 
    }
}