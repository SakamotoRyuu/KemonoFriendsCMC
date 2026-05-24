using UnityEngine;

public class ItemData {

    public int id;
    public int price;
    public Sprite image;
    public string path;
    public int serialNumber;

    public ItemData(int id, int price, Sprite image, string path) {
        this.id = id;
        this.price = price;
        this.image = image;
        this.path = path;
    }
}