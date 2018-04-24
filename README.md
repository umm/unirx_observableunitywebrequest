# UniRx ObservableUnityWebRequest

## What

* Wrapper for `UnityEngine.Networking.UnityWebRequest` with UniRx.

## Install

```shell
yarn add "umm-projects/unirx_observableunitywebrequest#^1.0.0"
```

## Usage

```csharp
using UniRx;

public class Sample {

    void Start() {
        ObservableUnityWebRequest.GetText("https://www.google.com/").Subscribe(
            (responseText) => {
                Debug.Log(responseText);
            }
        );
    }

}
```

* Provide some methods for defined in `UnityEngine.Networking.UnityWebRequest`.

## License

Copyright (c) 2017 Tetsuya Mori

Released under the MIT license, see [LICENSE.txt](LICENSE.txt)

