(function(f){if(typeof exports==="object"&&typeof module!=="undefined"){module.exports=f()}else if(typeof define==="function"&&define.amd){define([],f)}else{var g;if(typeof window!=="undefined"){g=window}else if(typeof global!=="undefined"){g=global}else if(typeof self!=="undefined"){g=self}else{g=this}g.adapter = f()}})(function(){var define,module,exports;return (function e(t,n,r){function s(o,u){if(!n[o]){if(!t[o]){var a=typeof require=="function"&&require;if(!u&&a)return a(o,!0);if(i)return i(o,!0);var f=new Error("Cannot find module '"+o+"'");throw f.code="MODULE_NOT_FOUND",f}var l=n[o]={exports:{}};t[o][0].call(l.exports,function(e){var n=t[o][1][e];return s(n?n:e)},l,l.exports,e,t,n,r)}return n[o].exports}var i=typeof require=="function"&&require;for(var o=0;o<r.length;o++)s(r[o]);return s})({1:[function(require,module,exports){

    /*
     *  Copyright (c) 2016 The WebRTC project authors. All Rights Reserved.
     *
     *  Use of this source code is governed by a BSD-style license
     *  that can be found in the LICENSE file in the root of the source
     *  tree.
     */
    /* eslint-env node */
    'use strict';

// Expose public methods.
    module.exports = {
      shimEnumerateDevices: require('./enumerateDevices')
    };

  },{"./enumerateDevices":2}],2:[function(require,module,exports){
    /*
     *  Copyright (c) 2016 The WebRTC project authors. All Rights Reserved.
     *
     *  Use of this source code is governed by a BSD-style license
     *  that can be found in the LICENSE file in the root of the source
     *  tree.
     */
    /* eslint-env node */
    'use strict';

// Expose public methods.
    module.exports = function() {
      if (!navigator.mediaDevices) {
        navigator.mediaDevices = {};
      }
      if (!navigator.mediaDevices.enumerateDevices) {
        navigator.mediaDevices.enumerateDevices = function() {
          return new Promise(function(resolve) {
            var kinds = {
              audio: 'audioinput',
              video: 'videoinput'
            };
            return MediaStreamTrack.getSources(function(devices) {
              resolve(devices.map(function(device) {
                return {
                  label: device.label,
                  kind: kinds[device.kind],
                  deviceId: device.id,
                  groupId: ''
                };
              }));
            });
          });
        };
      }
    };

  },{}],3:[function(require,module,exports){
    /*
     *  Copyright (c) 2016 The WebRTC project authors. All Rights Reserved.
     *
     *  Use of this source code is governed by a BSD-style license
     *  that can be found in the LICENSE file in the root of the source
     *  tree.
     */
    /* eslint-env node */
    'use strict';

    var browserDetails = require('../utils').browserDetails;

// Expose public methods.
    module.exports = function() {
      // Shim for mediaDevices on older versions.
      if (!navigator.mediaDevices) {
        navigator.mediaDevices = {
          addEventListener: function() {},
          removeEventListener: function() {}
        };
      }
      navigator.mediaDevices.enumerateDevices = navigator.mediaDevices.enumerateDevices || function() {
        return new Promise(function(resolve) {
          var infos = [
            {
              kind: 'audioinput',
              deviceId: 'default',
              label: '',
              groupId: ''
            },
            {
              kind: 'videoinput',
              deviceId: 'default',
              label: '',
              groupId: ''
            }
          ];
          resolve(infos);
        });
      };

      if (browserDetails.version < 41) {
        // Work around http://bugzil.la/1169665
        var orgEnumerateDevices = navigator.mediaDevices.enumerateDevices.bind(navigator.mediaDevices);
        navigator.mediaDevices.enumerateDevices = function() {
          return orgEnumerateDevices().then(undefined, function(e) {
            if (e.name === 'NotFoundError') {
              return [];
            }
            throw e;
          });
        };
      }
    };

  },{"../utils":6}],4:[function(require,module,exports){
    /*
     *  Copyright (c) 2016 The WebRTC project authors. All Rights Reserved.
     *
     *  Use of this source code is governed by a BSD-style license
     *  that can be found in the LICENSE file in the root of the source
     *  tree.
     */
    /* eslint-env node */
    'use strict';

// Expose public methods.
    module.exports = {
      shimEnumerateDevices: require('./enumerateDevices')
    };

  },{"./enumerateDevices":3}],5:[function(require,module,exports){
    /*
     *  Copyright (c) 2016 The WebRTC project authors. All Rights Reserved.
     *
     *  Use of this source code is governed by a BSD-style license
     *  that can be found in the LICENSE file in the root of the source
     *  tree.
     */
    /* eslint-env node */

    'use strict';

// Shimming starts here.
    (function() {
      // Utils.
      var logging = require('./utils').log;
      var browserDetails = require('./utils').browserDetails;
      // Export to the adapter global object visible in the browser.
      module.exports.browserDetails = browserDetails;
      module.exports.extractVersion = require('./utils').extractVersion;
      module.exports.disableLog = require('./utils').disableLog;

      // Comment out the line below if you want logging to occur, including logging
      // for the switch statement below. Can also be turned on in the browser via
      // adapter.disableLog(false), but then logging from the switch statement below
      // will not appear.
      require('./utils').disableLog(true);

      // Browser shims.
      var chromeShim = require('./chrome/chrome_shim') || null;
      var firefoxShim = require('./firefox/firefox_shim') || null;

      // Shim browser if found.
      switch (browserDetails.browser) {
        case 'opera': // fallthrough as it uses chrome shims
        case 'chrome':
          logging('enumerateDevices shimming chrome.');
          // Export to the adapter global object visible in the browser.
          module.exports.browserShim = chromeShim;
          chromeShim.shimEnumerateDevices();
          break;
        case 'firefox':
          logging('enumerateDevices shimming firefox.');
          // Export to the adapter global object visible in the browser.
          module.exports.browserShim = firefoxShim;
          firefoxShim.shimEnumerateDevices();
          break;
        default:
          logging('Unsupported browser!');
      }
    })();

  },{"./chrome/chrome_shim":1,"./firefox/firefox_shim":4,"./utils":6}],6:[function(require,module,exports){
// https://github.com/webrtc/adapter/blob/a117c4c1b4b8d86472a02f6be7b7e159b08d83aa/src/js/utils.js

    /*
     *  Copyright (c) 2016 The WebRTC project authors. All Rights Reserved.
     *
     *  Use of this source code is governed by a BSD-style license
     *  that can be found in the LICENSE file in the root of the source
     *  tree.
     */
    /* eslint-env node */
    'use strict';

    var logDisabled_ = false;

// Utility methods.
    var utils = {
      disableLog: function(bool) {
        if (typeof bool !== 'boolean') {
          return new Error('Argument type: ' + typeof bool +
            '. Please use a boolean.');
        }
        logDisabled_ = bool;
        return (bool) ? 'adapter.js logging disabled' :
          'adapter.js logging enabled';
      },

      log: function() {
        if (typeof window === 'object') {
          if (logDisabled_) {
            return;
          }
          if (typeof console !== 'undefined' && typeof console.log === 'function') {
            console.log.apply(console, arguments);
          }
        }
      },

      /**
       * Extract browser version out of the provided user agent string.
       *
       * @param {!string} uastring userAgent string.
       * @param {!string} expr Regular expression used as match criteria.
       * @param {!number} pos position in the version string to be returned.
       * @return {!number} browser version.
       */
      extractVersion: function(uastring, expr, pos) {
        var match = uastring.match(expr);
        return match && match.length >= pos && parseInt(match[pos], 10);
      },

      /**
       * Browser detector.
       *
       * @return {object} result containing browser, version and minVersion
       *     properties.
       */
      detectBrowser: function() {
        // Returned result object.
        var result = {};
        result.browser = null;
        result.version = null;
        result.minVersion = null;

        // Fail early if it's not a browser
        if (typeof window === 'undefined' || !window.navigator) {
          result.browser = 'Not a browser.';
          return result;
        }

        // Firefox.
        if (navigator.mozGetUserMedia) {
          result.browser = 'firefox';
          result.version = this.extractVersion(navigator.userAgent,
            /Firefox\/([0-9]+)\./, 1);
          result.minVersion = 31;

          // all webkit-based browsers
        } else if (navigator.webkitGetUserMedia) {
          // Chrome, Chromium, Webview, Opera, all use the chrome shim for now
          if (window.webkitRTCPeerConnection) {
            result.browser = 'chrome';
            result.version = this.extractVersion(navigator.userAgent,
              /Chrom(e|ium)\/([0-9]+)\./, 2);
            result.minVersion = 38;

            // Safari or unknown webkit-based
            // for the time being Safari has support for MediaStreams but not webRTC
          } else {
            // Safari UA substrings of interest for reference:
            // - webkit version:           AppleWebKit/602.1.25 (also used in Op,Cr)
            // - safari UI version:        Version/9.0.3 (unique to Safari)
            // - safari UI webkit version: Safari/601.4.4 (also used in Op,Cr)
            //
            // if the webkit version and safari UI webkit versions are equals,
            // ... this is a stable version.
            //
            // only the internal webkit version is important today to know if
            // media streams are supported
            //
            if (navigator.userAgent.match(/Version\/(\d+).(\d+)/)) {
              result.browser = 'safari';
              result.version = this.extractVersion(navigator.userAgent,
                /AppleWebKit\/([0-9]+)\./, 1);
              result.minVersion = 602;

              // unknown webkit-based browser
            } else {
              result.browser = 'Unsupported webkit-based browser ' +
                'with GUM support but no WebRTC support.';
              return result;
            }
          }

          // Edge.
        } else if (navigator.mediaDevices &&
          navigator.userAgent.match(/Edge\/(\d+).(\d+)$/)) {
          result.browser = 'edge';
          result.version = this.extractVersion(navigator.userAgent,
            /Edge\/(\d+).(\d+)$/, 2);
          result.minVersion = 10547;

          // Default fallthrough: not supported.
        } else {
          result.browser = 'Not a supported browser.';
          return result;
        }

        // Warn if version is less than minVersion.
        if (result.version < result.minVersion) {
          utils.log('Browser: ' + result.browser + ' Version: ' + result.version +
            ' < minimum supported version: ' + result.minVersion +
            '\n some things might not work!');
        }

        return result;
      }
    };

// Export.
    module.exports = {
      log: utils.log,
      disableLog: utils.disableLog,
      browserDetails: utils.detectBrowser(),
      extractVersion: utils.extractVersion
    };

  },{}]},{},[5])(5)
});