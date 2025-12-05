#!/usr/bin/env node
/*
  Build a single JS bundle (app.js) from all files under src/js using esbuild.
  - Recursively gathers all .js files under src/js/**
  - Concatenates in deterministic (path-sorted) order with file boundary comments
  - Emits wwwroot/js/app.js (minified, sourcemap, IIFE)
  - Relies on global jQuery/Bootstrap present at runtime
*/
const fs = require('fs');
const path = require('path');
const esbuild = require('esbuild');

const SRC_DIR = path.resolve(__dirname, '..', 'src', 'js');
const OUT_DIR = path.resolve(__dirname, '..', 'wwwroot', 'js');

function isDirectory(p) {
  try { return fs.statSync(p).isDirectory(); } catch { return false; }
}

function ensureDir(p) {
  if (!fs.existsSync(p)) fs.mkdirSync(p, { recursive: true });
}

function walkJsFiles(dir, acc = []) {
  if (!fs.existsSync(dir)) return acc;
  const entries = fs.readdirSync(dir, { withFileTypes: true });
  for (const ent of entries) {
    const full = path.join(dir, ent.name);
    if (ent.isDirectory()) {
      walkJsFiles(full, acc);
    } else if (ent.isFile() && ent.name.toLowerCase().endsWith('.js')) {
      acc.push(full);
    }
  }
  return acc;
}

async function buildApp() {
  const files = walkJsFiles(SRC_DIR)
    .sort((a, b) => a.localeCompare(b));

  if (files.length === 0) {
    console.warn('No .js files found in src/js — nothing to build.');
    return;
  }

  // Concatenate with file boundary comments to aid debugging
  let contents = '';
  for (const file of files) {
    const rel = path.relative(SRC_DIR, file).replace(/\\/g, '/');
    contents += `\n/* -- ${rel} -- */\n` + fs.readFileSync(file, 'utf8') + '\n';
  }

  ensureDir(OUT_DIR);
  const outFile = path.join(OUT_DIR, 'app.js');
  await esbuild.build({
    stdin: {
      contents,
      resolveDir: SRC_DIR,
      sourcefile: `app.concat.js`
    },
    bundle: false, // we just minify/transform the concatenated file
    minify: true,
    sourcemap: true,
    target: ['es2017'],
    format: 'iife',
    outfile: outFile,
    logLevel: 'info'
  });
}

async function main() {
  if (!isDirectory(SRC_DIR)) {
    console.error(`Missing source directory: ${SRC_DIR}`);
    process.exit(1);
  }
  await buildApp();
}

main().catch(err => {
  console.error(err);
  process.exit(1);
});
