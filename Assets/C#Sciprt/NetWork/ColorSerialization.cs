using ExitGames.Client.Photon;
using UnityEngine;

// 색상을 직렬화하는 클래스를 정의합니다.
public class ColorSerialization
{
    // 색상 데이터를 저장할 메모리 배열을 정의합니다. (4개의 float 값을 위한 공간: r, g, b, a)
    private static byte[] colorMemory = new byte[4 * 4];

    // 색상을 직렬화하여 StreamBuffer에 기록하는 메서드입니다.
    public static short SerializeColor(StreamBuffer outStream, object targetObject)
    {
        // targetObject를 Color로 변환합니다.
        Color color = (Color)targetObject;

        // 메모리 접근을 동기화합니다.
        lock (colorMemory)
        {
            byte[] bytes = colorMemory; // 색상 데이터를 저장할 배열을 참조합니다.
            int index = 0; // 배열 인덱스를 초기화합니다.

            // 색상의 각 구성 요소(r, g, b, a)를 직렬화합니다.
            Protocol.Serialize(color.r, bytes, ref index);
            Protocol.Serialize(color.g, bytes, ref index);
            Protocol.Serialize(color.b, bytes, ref index);
            Protocol.Serialize(color.a, bytes, ref index);
            // 직렬화된 바이트 배열을 StreamBuffer에 씁니다.
            outStream.Write(bytes, 0, 4 * 4);
        }

        // 직렬화된 데이터의 길이를 반환합니다.
        return 4 * 4;
    }

    // StreamBuffer에서 색상을 역직렬화하는 메서드입니다.
    public static object DeserializeColor(StreamBuffer inStream, short length)
    {
        Color color = new Color(); // 새로운 Color 객체를 생성합니다.

        // 메모리 접근을 동기화합니다.
        lock (colorMemory)
        {
            // StreamBuffer로부터 색상 데이터를 읽어옵니다.
            inStream.Read(colorMemory, 0, 4 * 4);
            int index = 0; // 배열 인덱스를 초기화합니다.

            // 배열로부터 색상의 각 구성 요소를 역직렬화합니다.
            Protocol.Deserialize(out color.r, colorMemory, ref index);
            Protocol.Deserialize(out color.g, colorMemory, ref index);
            Protocol.Deserialize(out color.b, colorMemory, ref index);
            Protocol.Deserialize(out color.a, colorMemory, ref index);
        }

        // 역직렬화된 Color 객체를 반환합니다.
        return color;
    }
}
