import * as esbuild from 'esbuild';

const keepFrameworkImports = {
    name: 'keep-framework-imports',
    setup(build) {
        build.onResolve({ filter: /_framework\// }, args => ({ path: args.path, external: true }));
    }
};

const options = {
    entryPoints: ['src/index.ts'],
    outfile: '../../wwwroot/js/index.js',
    bundle: true,
    format: 'esm',
    target: 'es2022',
    minify: true,
    plugins: [keepFrameworkImports]
};

if (process.argv.includes('--watch')) {
    const ctx = await esbuild.context(options);
    await ctx.watch();
} else {
    await esbuild.build(options);
}
