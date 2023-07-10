import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import { resolve } from "path";

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [vue()],
    build: {
        outDir: "out",
        lib: {
            entry: [
                resolve(__dirname, "main.ts"),
                resolve(__dirname, "components.ts"),
            ],
            name: "Vuez",
            fileName: "vuez",
        },
    },
    define: {
        "process.env": {},
    },
});
