using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MiniJSON;
using UnityEngine.Rendering;
using TMPro;

public class InstagramAPIIntergration : MonoBehaviour
{
    bool isUserProfileFetched = false;
    public GameObject instagramGameobjectPrefab;

    List<InstagramPictureObject> instagramDataList = new List<InstagramPictureObject>();

    public GameObject[] instagramPictures;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FetchInstagramPictures());
    }

    IEnumerator FetchInstagramPictures()
    {
        string url = "https://api.instagram.com/v1/users/self/media/recent/?access_token=506330496.3b119cf.a594cb94630540eabccc9c8c59f7477a";
        using(UnityWebRequest web_request = UnityWebRequest.Get(url))
        {
            yield return web_request.SendWebRequest();
            if (web_request.isNetworkError||web_request.isHttpError)
            {
                Debug.Log(web_request.error);
            }else
            {
                string api_response = web_request.downloadHandler.text;
                Debug.Log(api_response);

                IDictionary apiParse = (IDictionary)Json.Deserialize(api_response);
                IList InstagramPicturesList = (IList)apiParse["data"];

                foreach(IDictionary instagramPicture in InstagramPicturesList)
                {

                    //Getting the likes

                    IDictionary Likes = (IDictionary)instagramPicture["likes"];
                    long likes_count = (long)Likes["count"];

                    if (!isUserProfileFetched)
                    {
                        //Getting UserProfile info
                        IDictionary user = (IDictionary)instagramPicture["user"];
                        string userName = (string)user["username"];
                        string profilePicture_URL = (string)user["profile_picture"];

                        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(profilePicture_URL))
                        {
                            yield return uwr.SendWebRequest();
                            if (uwr.isNetworkError || uwr.isHttpError)
                            {
                                Debug.Log(uwr.error);
                            }
                            else
                            {
                                var profilePicturetexture = DownloadHandlerTexture.GetContent(uwr);
                                transform.Find("PortalMainParent").Find("ProfileInformation").Find("profilePicture").GetComponent<MeshRenderer>().material.mainTexture = profilePicturetexture;
                                transform.Find("PortalMainParent").Find("ProfileInformation").Find("userName").GetComponent<TextMeshPro>().text = userName+"'s";
                                isUserProfileFetched = true;
                             }
                        }
                    }

                



                    InstagramPictureObject instagramPictureObject= new InstagramPictureObject();
                    instagramPictureObject.instagramPictureData = instagramPicture;
                    instagramPictureObject.likes_count = likes_count;

                    //adding object to list
                    instagramDataList.Add(instagramPictureObject);



                    //IDictionary images = (IDictionary)instagramPicture["images"];
                    //IDictionary standardResolution = (IDictionary)images["standard_resolution"];
                    //string mainPicture_URL = (string)standardResolution["url"];

                    ////testing
                    //Debug.Log(mainPicture_URL);

                    //using(UnityWebRequest uwr=UnityWebRequestTexture.GetTexture(mainPicture_URL))
                    //{
                    //    yield return uwr.SendWebRequest();

                    //    if (uwr.isNetworkError||uwr.isHttpError)
                    //    {
                    //        Debug.Log(uwr.error);
                    //    }else
                    //    {
                    //        //get downloaded picture
                    //        var texture = DownloadHandlerTexture.GetContent(uwr);
                    //        GameObject instagramGameobject = Instantiate(instagramGameobjectPrefab);
                    //        instagramGameobject.transform.Find("MainPicture").GetComponent<MeshRenderer>().material.mainTexture = texture;



                    //    }
                    //}

                }
                instagramDataList.Sort((instagramPicture1,instagramPicture2) => -1*instagramPicture1.likes_count.CompareTo(instagramPicture2.likes_count));
                Debug.Log(instagramDataList[0].likes_count);
                Debug.Log(instagramDataList[1].likes_count);
                Debug.Log(instagramDataList[2].likes_count);
                Debug.Log(instagramDataList[3].likes_count);
                Debug.Log(instagramDataList[4].likes_count);
                PlaceInstagramPictures();
            }
        }

    }

    void PlaceInstagramPictures()
    {
        int i = 0;

        foreach (var instagramPicture in instagramPictures) 
        {
            StartCoroutine(InsertInstagramData(instagramPictures[i], instagramDataList[i]));
            i++;
         }

        //clear the list since it keeps a huge storage
        instagramDataList.Clear();
    }
    IEnumerator InsertInstagramData(GameObject instagramPicture, InstagramPictureObject instagramPictureObject)
    {
        IDictionary images = (IDictionary)instagramPictureObject.instagramPictureData["images"];
        IDictionary standardResolution = (IDictionary)images["standard_resolution"];
        string mainPicture_URL = (string)standardResolution["url"];

        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(mainPicture_URL))
        {
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                var texture = DownloadHandlerTexture.GetContent(uwr);
                instagramPicture.transform.Find("MainPicture").GetComponent<MeshRenderer>().material.mainTexture = texture;
                instagramPicture.transform.Find("likes_count").GetComponent<TextMeshPro>().text = instagramPictureObject.likes_count.ToString()+" likes";
            }
        }

    }

    public void UpdateInstagramPictureMaterials(bool areWeOutside)
    {
        foreach (var instagramPicture in instagramPictures)
        {
            if (areWeOutside==true)
            {
                //We are outside portal

               instagramPicture.transform.Find("MainPicture").GetComponent<MeshRenderer>().material.SetInt("stest", (int)CompareFunction.Equal);
                transform.Find("PortalMainParent").Find("ProfileInformation").localRotation = Quaternion.Euler(0, 0, 0);

            }
            else
            {
                //We are inside portal
                instagramPicture.transform.Find("MainPicture").GetComponent<MeshRenderer>().material.SetInt("stest", (int)CompareFunction.NotEqual);
                transform.Find("PortalMainParent").Find("ProfileInformation").localRotation = Quaternion.Euler(0, 180, 0);
            }

        }

    }

}


public class InstagramPictureObject
{
    public IDictionary instagramPictureData;
    public long likes_count;
}