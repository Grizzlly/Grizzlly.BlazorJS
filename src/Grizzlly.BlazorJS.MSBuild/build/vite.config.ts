import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import { resolve } from "path";
import { readFileSync } from "fs";

function isFileNotEmpty(filePath: string) {
    try {
        // Read the file synchronously
        const fileContent = readFileSync(filePath, "utf8");

        // Check if the file content is empty (has zero length)
        return fileContent.length > 0;
    } catch (error) {
        // An error occurred (file not found or other issue)
        return false;
    }
}

// only include imports.js in entry point if it exists
const importsPath = resolve(__dirname, "imports.js");
let entries = [resolve(__dirname, "main.ts")];
if (isFileNotEmpty(importsPath)) {
    entries.push(importsPath);
}

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [vue()],
    build: {
        outDir: "out",
        lib: {
            entry: entries,
            name: "Grizzlly.BlazorJS",
            fileName: "Grizzlly.BlazorJS",
        },
    },
    define: {
        "process.env": {},
    },
});
