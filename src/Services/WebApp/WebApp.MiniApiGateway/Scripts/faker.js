/**
 * Skipped minification because the original files appears to be already minified.
 * Original file: /npm/@faker-js/faker@8.3.1/dist/cjs/index.js
 *
 * Do NOT use SRI with dynamically generated files! More information: https://www.jsdelivr.com/using-sri-with-dynamic-files
 */
"use strict";var M=Object.create;var s=Object.defineProperty;var C=Object.getOwnPropertyDescriptor;var S=Object.getOwnPropertyNames;var F=Object.getPrototypeOf,h=Object.prototype.hasOwnProperty;var k=(e,o)=>{for(var i in o)s(e,i,{get:o[i],enumerable:!0})},m=(e,o,i,l)=>{if(o&&typeof o=="object"||typeof o=="function")for(let r of S(o))!h.call(e,r)&&r!==i&&s(e,r,{get:()=>o[r],enumerable:!(l=C(o,r))||l.enumerable});return e},t=(e,o,i)=>(m(e,o,"default"),i&&m(i,o,"default")),P=(e,o,i)=>(i=e!=null?M(F(e)):{},m(o||!e||!e.__esModule?s(i,"default",{value:e,enumerable:!0}):i,e)),A=e=>m(s({},"__esModule",{value:!0}),e);var n={};k(n,{Aircraft:()=>c.Aircraft,CssFunction:()=>a.CssFunction,CssSpace:()=>a.CssSpace,Faker:()=>D.Faker,FakerError:()=>p.FakerError,Sex:()=>y.Sex,SimpleFaker:()=>f.SimpleFaker,allLocales:()=>u,faker:()=>d.fakerEN,mergeLocales:()=>x.mergeLocales,simpleFaker:()=>f.simpleFaker});module.exports=A(n);var p=require("./errors/faker-error"),D=require("./faker");t(n,require("./locale"),module.exports);var d=require("./locale");t(n,require("./locales"),module.exports);var u=P(require("./locales")),c=require("./modules/airline"),a=require("./modules/color"),y=require("./modules/person"),f=require("./simple-faker"),x=require("./utils/merge-locales");0&&(module.exports={Aircraft,CssFunction,CssSpace,Faker,FakerError,Sex,SimpleFaker,allLocales,faker,mergeLocales,simpleFaker,...require("./locale"),...require("./locales")});