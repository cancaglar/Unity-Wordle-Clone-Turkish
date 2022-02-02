using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Globalization;

public class GameManager : MonoBehaviour
{
    [SerializeField] List<Text> textsCollumn1 = new List<Text>();
    [SerializeField] List<Text> textsCollumn2 = new List<Text>();
    [SerializeField] List<Text> textsCollumn3 = new List<Text>();
    [SerializeField] List<Text> textsCollumn4 = new List<Text>();
    [SerializeField] List<Text> textsCollumn5 = new List<Text>();

    public bool first;
    public bool second;
    public bool third;
    public bool fourth;
    public bool fifth;
    string currentWord;

    public Sprite GreenLetter;
    public Sprite YellowLetter;
    public Sprite GrayLetter;

    public RectTransform WinPanel;
    public RectTransform LosePanel;
    [SerializeField] private float uiX = 400f;
    [SerializeField] private float uiY = 0f;
    [SerializeField] private float uiDuration = 0.25f;
    [SerializeField] ParticleSystem confetti;

    Color32 keyboardGreen = new Color32(83, 141, 78, 255);
    Color32 keyboardGray = new Color32(58, 58, 60, 255);
    Color32 keyboardYellow = new Color32(181, 159, 59, 255);
    [SerializeField] Text currentWordText;
    void Start()
    {
        currentWord = Words.GetWord();
        Debug.Log(currentWord);
        confetti.Stop();
        currentWordText.text = currentWord;
    }

    public void LetterButton(string letter)
    {
        if (!first) // means we didnt finish the first column
        {
            WriteTheLetter(textsCollumn1, letter);
        }
        else if (!second)
        {
            WriteTheLetter(textsCollumn2, letter);
        }
        else if (!third)
        {
            WriteTheLetter(textsCollumn3, letter);
        }
        else if (!fourth)
        {
            WriteTheLetter(textsCollumn4, letter);
        }
        else if (!fifth)
        {
            WriteTheLetter(textsCollumn5, letter);
        }

    }
    private void WriteTheLetter(List<Text> list, string letter)
    {
        for (var i = 0; i < list.Count; i++)
        {
            if (string.IsNullOrEmpty(list[i].text))
            {
                list[i].text = letter;
                break;
            }
        }
    }

    public void EnterButton()
    {
        if (!first)
        {
            WordCheck(textsCollumn1, ref first);
        }
        else if (!second)
        {
            WordCheck(textsCollumn2, ref second);
        }
        else if (!third)
        {
            WordCheck(textsCollumn3, ref third);
        }
        else if (!fourth)
        {
            WordCheck(textsCollumn4, ref fourth);
        }
        else if (!fifth)
        {
            WordCheck(textsCollumn5, ref fifth);
        }
    }
    private void WordCheck(List<Text> list, ref bool check)
    {
        int count = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (string.IsNullOrEmpty(list[i].text))
            {
                // shake letters
                list[i].rectTransform.DOShakeAnchorPos(1f, 5f, 10, 1);
                break; // there is still have some empty cells, do nothing
            }
            else
            {
                count += 1;
                if (count >= 5)
                {
                    if (Words.IsItOnTheList(GetColumnsWord(list)))
                    {
                        // its in the list
                        if (currentWord == GetColumnsWord(list))
                        {
                            // Win and green letters
                            //Debug.Log("Win");
                            CheckLetter(list, list[i], GetColumnsWord(list));
                            Win();
                        }
                        else
                        {
                            // its in the list but not the current word
                            // check for letters
                            //Debug.Log("its in the list but not not the current word");
                            check = true;
                            CheckLetter(list, list[i], GetColumnsWord(list));
                            if (fifth)
                            {
                                Lose();
                            }
                        }
                    }
                    else
                    {
                        // its not in the list
                        // shake letters
                        //Debug.Log("its not in the list");
                        for (int a = 0; a < list.Count; a++)
                        {
                            list[a].rectTransform.DOShakeAnchorPos(1f, 5f, 10, 1);
                        }
                    }

                    count = 0;
                }
            }
        }
    }

    public string GetColumnsWord(List<Text> column)
    {
        string word = "";
        for (int i = 0; i < column.Count; i++)
        {
            word += column[i].text;
        }
        return word.ToLower(new CultureInfo("tr-TR", false));
    }
    public void DeleteButton()
    {
        if (!first)
        {
            DeleteTheLetter(textsCollumn1);
        }
        else if (!second)
        {
            DeleteTheLetter(textsCollumn2);
        }
        else if (!third)
        {
            DeleteTheLetter(textsCollumn3);
        }
        else if (!fourth)
        {
            DeleteTheLetter(textsCollumn4);
        }
        else if (!fifth)
        {
            DeleteTheLetter(textsCollumn5);
        }
    }
    void DeleteTheLetter(List<Text> list)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (!string.IsNullOrEmpty(list[i].text))
            {
                list[i].text = "";
                break;
            }
        }
    }

    void CheckLetter(List<Text> currentList, Text txt, string word)
    {
        for (int i = 0; i < word.Length; i++)
        {
            bool result = currentWord.Contains(word[i].ToString().ToLower(new CultureInfo("tr-TR", false)));
            if (result)
            {
                // check for letter position

                if (word[i] == currentWord[i]) // same letters means same column
                {
                    // same column green
                    currentList[i].gameObject.transform.parent.gameObject.GetComponent<Image>().sprite = GreenLetter;
                    GameObject.Find(currentList[i].text).GetComponent<Image>().color = keyboardGreen;
                }
                else
                {
                    // diffrent collumn Yellow
                    currentList[i].gameObject.transform.parent.gameObject.GetComponent<Image>().sprite = YellowLetter;
                    if (GameObject.Find(currentList[i].text).GetComponent<Image>().color != keyboardGreen)
                    {
                        GameObject.Find(currentList[i].text).GetComponent<Image>().color = keyboardYellow;
                    }
                    
                }
            }
            else
            {
                currentList[i].gameObject.transform.parent.gameObject.GetComponent<Image>().sprite = GrayLetter;
                GameObject.Find(currentList[i].text).GetComponent<Image>().color = keyboardGray;
            }
        }


    }
    void Win()
    {
        WinPanel.DOAnchorPos(new Vector2(uiX, uiY), uiDuration).OnStepComplete(() => { confetti.Play(); });
    }
    void Lose()
    {
        LosePanel.DOAnchorPos(new Vector2(uiX, uiY), uiDuration);
    }
    public void PlayAgainButton()
    {
        SceneManager.LoadScene(0);
    }


}
