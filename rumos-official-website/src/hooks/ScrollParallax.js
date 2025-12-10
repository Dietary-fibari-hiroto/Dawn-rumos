export function scrollParallax(selector = ".tracked-element", ratio = 0.2) {
  if (typeof window === "undefined") return;

  const elements = Array.from(document.querySelectorAll(selector));

  const handleScroll = () => {
    const scrollY = window.scrollY;
    const windowHeight = window.innerHeight;

    elements.forEach((el) => {
      const rect = el.getBoundingClientRect();
      const offsetTop = rect.top + scrollY;

      if (
        scrollY + windowHeight > offsetTop &&
        scrollY < offsetTop + rect.height
      ) {
        const offset = (scrollY - offsetTop + windowHeight / 2) * ratio;
        el.style.transform = `translateY(${offset}px)`;
      }
    });
  };

  window.addEventListener("scroll", handleScroll);
  handleScroll();

  // クリーンアップ用
  return () => window.removeEventListener("scroll", handleScroll);
}

export function initMultiParallax(configs) {
  if (typeof window === "undefined") return;

  const elements = configs.map(({ selector, ratio }) => ({
    nodes: Array.from(document.querySelectorAll(selector)),
    ratio,
  }));

  const handleScroll = () => {
    const scrollY = window.scrollY;
    const windowHeight = window.innerHeight;

    elements.forEach(({ nodes, ratio }) => {
      nodes.forEach((el) => {
        const rect = el.getBoundingClientRect();
        const offsetTop = rect.top + scrollY;

        if (
          scrollY + windowHeight > offsetTop &&
          scrollY < offsetTop + rect.height
        ) {
          const offset = (scrollY - offsetTop + windowHeight / 2) * ratio;
          el.style.transform = `translateY(${offset}px)`;
        }
      });
    });
  };

  window.addEventListener("scroll", handleScroll);
  handleScroll();

  // クリーンアップ
  return () => window.removeEventListener("scroll", handleScroll);
}
