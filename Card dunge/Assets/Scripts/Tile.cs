using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Tile : MonoBehaviour
{
    public TileState state { get; private set; }
    public TileCell cell { get; private set; }
    public int number { get; private set; }
    public float elapsed = 1.0f;
    public float duration = 0.1f;

    private Image background;
    private TextMeshProUGUI text;


    private void Awake()
    {
        background = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetState(TileState state, int number)
    {
        this.state = state;
        this.number = number;

        background.color = state.backgroundColor;
        text.color = state.textColor;
        text.text = number.ToString();

    }

    public void Spawn(TileCell cell)
    {

        if (this.cell != null)
        {
            this.cell.Tile = null;
        }
        this.cell = cell;
        this.cell.Tile = this;

        transform.position = cell.transform.position;
    }

    public void MoveTo(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.Tile = null;
        }

        this.cell = cell;
        this.cell.Tile = this;

        StartCoroutine(Animate(cell.transform.position, false));
    }

    public void Merge(TileCell cell){
        if (this.cell != null){
            this.cell.Tile = null;
        }

        this.cell = null;
        StartCoroutine(Animate(cell.transform.position, true));
    }

    private IEnumerator Animate(Vector3 to, bool merging)
    {
        Vector3 from = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = to;

        if(merging){
            Destroy(gameObject);
        }
    }
}


