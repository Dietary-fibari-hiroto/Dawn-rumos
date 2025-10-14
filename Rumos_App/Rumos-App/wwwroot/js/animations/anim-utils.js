//ロード時のフェードインアニメーション
export const loaded_FadeIn = () => {
    //非同期処理
    return new Promise((resolve) => {
        const item = document.querySelector(".loaded_fadein");
        if (!item) { //itemが存在しなければ何もしない
            resolve(false);
            return;
        }

        //showクラスを付与
            item.classList.add("show");
        setTimeout(() => {
                //transisionが終わったときの処理を設定
                item.addEventListener("transitionend", () => {
                    resolve(true);//promiseが完了したかどうかを返す
                }, { once: true });
                //showクラスを奪取
                item.classList.remove("show");
            }, 2000)


    });
}

