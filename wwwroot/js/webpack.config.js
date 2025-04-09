const path = require('path');

module.exports = {
  entry: {
    'lecturer-chat': './wwwroot/js/lecturer_chat.js',
    'student-chat': './wwwroot/js/SignalR.js'
  },
  output: {
    filename: '[name].bundle.js',
    path: path.resolve(__dirname, 'wwwroot/js/dist')
  },
  module: {
    rules: [
      {
        test: /\.js$/,
        exclude: /node_modules/,
        use: {
          loader: 'babel-loader',
          options: {
            presets: ['@babel/preset-env']
          }
        }
      }
    ]
  }
};