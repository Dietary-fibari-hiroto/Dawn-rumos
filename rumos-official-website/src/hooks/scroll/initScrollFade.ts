export interface ScrollFadeOptions {
  selector?: string; //対象要素のセレクタ
  threshold?: number; //表示判定の閾値
  rootMargin?: string; //判定エリアのマージン
  showClass?: string; //表示時に追加するクラス
  hideClass?: string; // 非表示時に追加するクラス
  once?: boolean; //一度だけ表示するか
  delay?: number; //遅延時間
  stagger?: number; //複数要素の遅延間隔
  hideOnPassThrough?: boolean; //スクロールで通り過ぎたら非表示にするか
}

export const initScrollFade = (options: ScrollFadeOptions = {}) => {
  const {
    selector = ".scroll-fade",
    threshold = 0.2,
    rootMargin = "0px 0px 0% 0px",
    showClass = "show",
    hideClass = "",
    once = false,
    delay = 0,
    stagger = 0,
    hideOnPassThrough = false,
  } = options;

  const elements = document.querySelectorAll(selector);

  if (elements.length === 0) {
    console.warn(`No elements found with selector: ${selector}`);
    return;
  }

  const observer = new IntersectionObserver(
    (entries) => {
      entries.forEach((entry) => {
        const target = entry.target as HTMLElement;
        const rect = target.getBoundingClientRect();
        const hasPassedThrough = rect.bottom < 0; //画面上部を通り過ぎた
        const isBelowViewport = rect.top > window.innerHeight; //まだ下にある

        if (entry.isIntersecting) {
          //画面内に入った
          const elementDelay =
            delay + stagger * parseInt(target.dataset.index || "0");

          setTimeout(() => {
            target.classList.add(showClass);
            if (hideClass) {
              target.classList.remove(hideClass);
            }
            //一度表示したことをマーク
            target.dataset.hasShown = "true";
          }, elementDelay);

          //一度だけ表示する場合は監視を解除
          if (once) {
            observer.unobserve(entry.target);
          }
        } else {
          //画面外に出た
          const hasShown = target.dataset.hasShown === "true";

          if (!once) {
            if (hideOnPassThrough) {
              //通り過ぎたら非表示、下にある場合も非表示
              target.classList.remove(showClass);
              if (hideClass) {
                target.classList.add(hideClass);
              }
            } else {
              //hideOnPassThrough が false の場合
              if (hasPassedThrough && hasShown) {
                //通り過ぎても表示を維持
                //何もしない（showクラスを残す）
              } else {
                //それ以外（下にある、または未表示）は非表示
                target.classList.remove(showClass);
                if (hideClass) {
                  target.classList.add(hideClass);
                }
              }
            }
          }
        }
      });
    },
    {
      threshold,
      rootMargin,
    }
  );

  //各要素を監視
  elements.forEach((element, index) => {
    //stagger用のインデックスを設定
    (element as HTMLElement).dataset.index = index.toString();
    observer.observe(element);
  });

  //クリーンアップ関数を返す
  return () => {
    observer.disconnect();
  };
};
