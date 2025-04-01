import { nodeResolve } from "@rollup/plugin-node-resolve";

export default {
  input: "Student_chat.js",
  output: {
    file: "dist/Student_chat.js",
    format: "iife",
    sourcemap: true,
  },
  plugins: [nodeResolve()],
};
