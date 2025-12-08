import { useState, useEffect, useRef } from "react";
import { motion, AnimatePresence } from "framer-motion";

export function FadeInOnScrollHero({ children }) {
  const [isShow, setIsShow] = useState(false);

  const animationSetting = {
    initial: { y: 100, opacity: 0 },
    animate: { y: 0, opacity: 1 },
    exit: { y: 100, opacity: 0 },
    transition: { duration: 1, ease: "easeInOut" },
  };

  useEffect(() => {
    const handleScroll = () => {
      if (window.scrollY > 1000) setIsShow(true);
      else setIsShow(false);
    };
    window.addEventListener("scroll", handleScroll);
    return () => window.removeEventListener("scroll", handleScroll);
  }, []);

  return (
    <div style={{ overflow: "hidden" }}>
      <AnimatePresence mode="wait">
        {isShow && (
          <motion.div key="hero" {...animationSetting}>
            {children}
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  );
}

export function ElementOnScroll({ children }) {
  const [isShow, setIsShow] = useState(false);
  const ref = useRef(null);

  useEffect(() => {
    const handleScroll = () => {
      if (!ref.current) return;
      const rect = ref.current.getBoundingClientRect();
      if (rect.top < window.innerHeight * 0.8) setIsShow(true);
      else setIsShow(false);
    };

    window.addEventListener("scroll", handleScroll);
    return () => window.removeEventListener("scroll", handleScroll);
  }, []);

  return (
    <div ref={ref} className={`scroll-fade ${isShow ? "show" : ""}`}>
      {children}
    </div>
  );
}

export function MotionView({ children }) {
  return (
    <AnimatePresence mode="wait">
      <motion.div
        key="yysgh"
        initial={{ opacity: 0, y: 50 }}
        whileInView={{ opacity: 1, y: 0 }}
        transition={{ duration: 2, ease: "easeInOut" }}
        viewport={{ amount: 0.2 }} // 30%見えたら発火、一度だけ
      >
        {children}
      </motion.div>
    </AnimatePresence>
  );
}
