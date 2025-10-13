export const selectionImageChanging = () => {
    const scroller = document.querySelector("#scroller");
    const panels = document.querySelectorAll(".panel");
    const bg = document.querySelector(".background-image");
    const menuItems = document.querySelectorAll(".menu p");
    // 背景画像一覧
    const backgrounds = [
      "images/photos/IMG_7053.jpg",
      "images/photos/IMG_3681.jpg",
      "images/photos/IMG_7083.jpg",
      "images/photos/Environment_bg.png",
      "images/photos/IMG_7067.jpg"
    ];

    let currentIndex = 0;//動作カウント
    bg.src = backgrounds[0];//初期画像

    scroller.addEventListener("scroll", () => {
        const scrollTop = scroller.scrollTop; //scroller内でのスクロール量
        const pageHeight = window.innerHeight; //ビューポートの高さ(子要素の高さと一致するためそのまま使用)
        const pageIndex = Math.round(scrollTop / pageHeight); //何個目のセクションにいるか

        if (pageIndex !== currentIndex && pageIndex < backgrounds.length) { //カレントと現在値が一致しないときと、画像数の範囲内の時
            fadeBackground(pageIndex);
            updateMenu(pageIndex);
            currentIndex = pageIndex; //処理が終わったら現在のIndexを過去のIndexとして反映
        }

    });

    /**
     * バックグラウンドを更新する関数
     * @param {any} index
     */
    function fadeBackground(index) {
        bg.style.opacity = 0;
        setTimeout(() => {
            bg.src = backgrounds[index];
            bg.style.opacity = 0.8;
        }, 400);
    }

    /**
     * 現行のメニューテキストの色を返る関数
     * @param {any} index
     */
    function updateMenu(index) {
        menuItems.forEach((item, i) => {
            item.classList.toggle("active", i === index); //iの要素にだけactiveを付与
        });
    }
}

