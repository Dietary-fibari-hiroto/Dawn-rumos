import { useEffect } from "react";

export const useScrollParallax = (
  selector = ".tracked-element",
  ratio = 0.2
) => {
  useEffect(() => {
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

    return () => {
      window.removeEventListener("scroll", handleScroll);
    };
  }, [selector, ratio]);
};

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
  return () => window.removeEventListener("scroll", handleScroll);
}
