// Minifies the built CSS and JavaScript in dist/. A file that fails to minify is left as it is.
import { readdir, readFile, writeFile } from "node:fs/promises";
import { join, extname } from "node:path";
import CleanCSS from "clean-css";
import { minify } from "terser";

const DIST = "dist";
const css = new CleanCSS({ level: 2 });

async function* files(dir) {
  for (const entry of await readdir(dir, { withFileTypes: true })) {
    const path = join(dir, entry.name);
    if (entry.isDirectory()) yield* files(path);
    else yield path;
  }
}

let before = 0;
let after = 0;
for await (const path of files(DIST)) {
  const ext = extname(path);
  if (ext !== ".css" && ext !== ".js") continue;
  if (path.includes(".min.")) continue;
  const source = await readFile(path, "utf8");
  try {
    const output = ext === ".css" ? css.minify(source).styles : (await minify(source)).code;
    if (!output) throw new Error("empty output");
    await writeFile(path, output);
    before += source.length;
    after += output.length;
  } catch (error) {
    console.warn(`[optimize] kept ${path} as it is: ${error.message}`);
  }
}
console.log(`[optimize] CSS and JS ${(before / 1024).toFixed(1)} KB -> ${(after / 1024).toFixed(1)} KB`);
