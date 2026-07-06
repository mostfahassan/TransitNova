window.tailwind = window.tailwind || {};
window.tailwind.config = {
    theme: {
        extend: {
            fontFamily: {
                sans: ["Inter", "ui-sans-serif", "system-ui", "Segoe UI", "sans-serif"],
                mono: ["IBM Plex Mono", "SFMono-Regular", "Consolas", "Liberation Mono", "monospace"]
            },
            colors: {
                brand: {
                    canvas: "#F7F6F3",
                    paper: "#FFFFFF",
                    ink: "#0F172A",
                    muted: "#475569",
                    line: "#E2E8F0",
                    blue: "#2563EB",
                    blueDark: "#1D4ED8",
                    blueSoft: "#EFF6FF",
                    steel: "#CBD5E1"
                },
                account: {
                    bg: "#F8FAFC",
                    panel: "#FFFFFF",
                    ink: "#0F172A",
                    muted: "#475569",
                    line: "#E2E8F0",
                    primary: "#2563EB",
                    primaryDark: "#1D4ED8"
                },
                admin: {
                    bg: "#F7F6F3",
                    paper: "#FBFBFA",
                    card: "#FFFFFF",
                    ink: "#111111",
                    muted: "#2F3437",
                    soft: "#787774",
                    line: "#EAEAEA",
                    blue: "#2563EB"
                },
                dashboard: {
                    bg: "#F8FAFC",
                    shell: "#FFFFFF",
                    ink: "#0F172A",
                    muted: "#475569",
                    line: "#E2E8F0",
                    primary: "#2563EB",
                    primaryDark: "#1D4ED8",
                    success: "#16A34A",
                    warning: "#D97706",
                    danger: "#DC2626"
                }
            },
            boxShadow: {
                soft: "0 18px 60px rgba(15, 23, 42, 0.08)",
                precise: "0 1px 0 rgba(15, 23, 42, 0.08), 0 24px 70px rgba(15, 23, 42, 0.08)"
            },
            transitionTimingFunction: {
                smooth: "cubic-bezier(0.16, 1, 0.3, 1)"
            },
            letterSpacing: {
                wideTech: "0.12em"
            }
        }
    }
};



