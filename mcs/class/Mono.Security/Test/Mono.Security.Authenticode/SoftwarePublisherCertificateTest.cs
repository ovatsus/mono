//
// SoftwarePublisherCertificateTest.cs 
//	- NUnit Test Cases for Software Publisher Certificate
//
// Author:
//	Sebastien Pouliot (sebastien@ximian.com)
//
// (C) 2003 Motus Technologies Inc. (http://www.motus.com)
// Copyright (C) 2004 Novell (http://www.novell.com)
//

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using Mono.Security.Authenticode;
using MSX = Mono.Security.X509;

using NUnit.Framework;

namespace MonoTests.Mono.Security.Authenticode {

	// HOWTO create a SPC file
	// cert2spc cert1.cer cacert.cer cacrl.crl ... output.spc
	
	[TestFixture]
	public class SoftwarePublisherCertificateFileTest {
	
		static byte[] certonly = { 
		0x30, 0x82, 0x03, 0x1E, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 
		0x01, 0x07, 0x02, 0xA0, 0x82, 0x03, 0x0F, 0x30, 0x82, 0x03, 0x0B, 0x02, 
		0x01, 0x01, 0x31, 0x00, 0x30, 0x0B, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 
		0xF7, 0x0D, 0x01, 0x07, 0x01, 0xA0, 0x82, 0x02, 0xF3, 0x30, 0x82, 0x02, 
		0xEF, 0x30, 0x82, 0x02, 0x5A, 0xA0, 0x03, 0x02, 0x01, 0x02, 0x02, 0x01, 
		0x6C, 0x30, 0x0B, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 
		0x01, 0x05, 0x30, 0x56, 0x31, 0x0B, 0x30, 0x09, 0x06, 0x03, 0x55, 0x04, 
		0x06, 0x13, 0x02, 0x55, 0x53, 0x31, 0x18, 0x30, 0x16, 0x06, 0x03, 0x55, 
		0x04, 0x0A, 0x13, 0x0F, 0x55, 0x2E, 0x53, 0x2E, 0x20, 0x47, 0x6F, 0x76, 
		0x65, 0x72, 0x6E, 0x6D, 0x65, 0x6E, 0x74, 0x31, 0x0C, 0x30, 0x0A, 0x06, 
		0x03, 0x55, 0x04, 0x0B, 0x13, 0x03, 0x44, 0x6F, 0x44, 0x31, 0x0D, 0x30, 
		0x0B, 0x06, 0x03, 0x55, 0x04, 0x0B, 0x13, 0x04, 0x4E, 0x61, 0x76, 0x79, 
		0x31, 0x10, 0x30, 0x0E, 0x06, 0x03, 0x55, 0x04, 0x03, 0x13, 0x07, 0x4E, 
		0x61, 0x76, 0x79, 0x20, 0x43, 0x41, 0x30, 0x1E, 0x17, 0x0D, 0x30, 0x32, 
		0x31, 0x30, 0x31, 0x31, 0x31, 0x33, 0x31, 0x32, 0x35, 0x30, 0x5A, 0x17, 
		0x0D, 0x30, 0x34, 0x31, 0x30, 0x31, 0x30, 0x31, 0x33, 0x31, 0x32, 0x35, 
		0x30, 0x5A, 0x30, 0x81, 0x8B, 0x31, 0x0B, 0x30, 0x09, 0x06, 0x03, 0x55, 
		0x04, 0x06, 0x13, 0x02, 0x55, 0x53, 0x31, 0x18, 0x30, 0x16, 0x06, 0x03, 
		0x55, 0x04, 0x0A, 0x13, 0x0F, 0x55, 0x2E, 0x53, 0x2E, 0x20, 0x47, 0x6F, 
		0x76, 0x65, 0x72, 0x6E, 0x6D, 0x65, 0x6E, 0x74, 0x31, 0x0C, 0x30, 0x0A, 
		0x06, 0x03, 0x55, 0x04, 0x0B, 0x13, 0x03, 0x44, 0x6F, 0x44, 0x31, 0x0D, 
		0x30, 0x0B, 0x06, 0x03, 0x55, 0x04, 0x0B, 0x13, 0x04, 0x4E, 0x61, 0x76, 
		0x79, 0x31, 0x12, 0x30, 0x10, 0x06, 0x03, 0x55, 0x04, 0x0B, 0x13, 0x09, 
		0x6C, 0x6F, 0x63, 0x61, 0x74, 0x69, 0x6F, 0x6E, 0x73, 0x31, 0x1B, 0x30, 
		0x19, 0x06, 0x03, 0x55, 0x04, 0x07, 0x13, 0x12, 0x41, 0x6E, 0x6E, 0x61, 
		0x70, 0x6F, 0x6C, 0x69, 0x73, 0x20, 0x4A, 0x75, 0x6E, 0x63, 0x74, 0x69, 
		0x6F, 0x6E, 0x31, 0x14, 0x30, 0x12, 0x06, 0x03, 0x55, 0x04, 0x03, 0x13, 
		0x0B, 0x4E, 0x61, 0x76, 0x79, 0x20, 0x55, 0x73, 0x65, 0x72, 0x20, 0x31, 
		0x30, 0x81, 0x9F, 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 
		0x0D, 0x01, 0x01, 0x01, 0x05, 0x00, 0x03, 0x81, 0x8D, 0x00, 0x30, 0x81, 
		0x89, 0x02, 0x81, 0x81, 0x00, 0xB7, 0x7E, 0x94, 0x5F, 0xE8, 0x2A, 0xE7, 
		0xAD, 0x82, 0x16, 0x2C, 0x3D, 0x2F, 0x5E, 0x88, 0x67, 0xF0, 0x23, 0x26, 
		0x15, 0x34, 0x04, 0x1F, 0x63, 0x8B, 0xFE, 0xFB, 0xBB, 0x0D, 0xC0, 0x7E, 
		0xF0, 0x46, 0x82, 0x09, 0xA2, 0x91, 0xE0, 0xEA, 0xEF, 0xD0, 0x43, 0xCB, 
		0x30, 0x45, 0xAC, 0x7C, 0xAC, 0xFC, 0xBE, 0x54, 0x79, 0x77, 0xA9, 0x6A, 
		0x45, 0xF5, 0xBF, 0xE5, 0xEF, 0x97, 0x11, 0x63, 0xC2, 0xF7, 0x3C, 0x73, 
		0x6D, 0xBA, 0x8D, 0xFE, 0xAE, 0x28, 0x4A, 0x29, 0xE4, 0xA2, 0x59, 0x0C, 
		0x8F, 0x1A, 0x57, 0x86, 0xF2, 0x42, 0xF7, 0x35, 0x0B, 0xC3, 0xA5, 0x31, 
		0xD8, 0x19, 0xE2, 0x97, 0x7A, 0xA1, 0xF4, 0xE5, 0xDB, 0xCA, 0xF5, 0x54, 
		0x39, 0x1D, 0x0E, 0xDF, 0x78, 0x73, 0xBF, 0x86, 0x97, 0x40, 0xAA, 0x06, 
		0x8E, 0x8B, 0x6B, 0x0C, 0x06, 0x98, 0xD7, 0xD2, 0x1D, 0x45, 0xAA, 0x7F, 
		0xA5, 0x02, 0x03, 0x01, 0x00, 0x01, 0xA3, 0x81, 0x9A, 0x30, 0x81, 0x97, 
		0x30, 0x1F, 0x06, 0x03, 0x55, 0x1D, 0x23, 0x04, 0x18, 0x30, 0x16, 0x80, 
		0x14, 0xFB, 0x96, 0xF0, 0x10, 0xC4, 0x37, 0x55, 0xF0, 0xCE, 0xB5, 0xA6, 
		0xE2, 0xF1, 0x19, 0xFF, 0x99, 0x1A, 0xAE, 0x6E, 0x58, 0x30, 0x1D, 0x06, 
		0x03, 0x55, 0x1D, 0x0E, 0x04, 0x16, 0x04, 0x14, 0x02, 0x48, 0x78, 0xB9, 
		0xCC, 0x01, 0x51, 0x31, 0x74, 0x7F, 0x39, 0x2A, 0x37, 0xC2, 0x44, 0x93, 
		0x7E, 0x98, 0x69, 0x80, 0x30, 0x0B, 0x06, 0x03, 0x55, 0x1D, 0x0F, 0x04, 
		0x04, 0x03, 0x02, 0x04, 0xF0, 0x30, 0x17, 0x06, 0x03, 0x55, 0x1D, 0x20, 
		0x04, 0x10, 0x30, 0x0E, 0x30, 0x0C, 0x06, 0x0A, 0x60, 0x86, 0x48, 0x01, 
		0x65, 0x03, 0x02, 0x01, 0x30, 0x01, 0x30, 0x2F, 0x06, 0x03, 0x55, 0x1D, 
		0x11, 0x04, 0x28, 0x30, 0x26, 0x81, 0x24, 0x4E, 0x61, 0x76, 0x79, 0x31, 
		0x40, 0x77, 0x61, 0x72, 0x72, 0x65, 0x6E, 0x74, 0x6F, 0x6E, 0x2E, 0x61, 
		0x74, 0x6C, 0x2E, 0x67, 0x65, 0x74, 0x72, 0x6F, 0x6E, 0x69, 0x63, 0x73, 
		0x67, 0x6F, 0x76, 0x2E, 0x63, 0x6F, 0x6D, 0x30, 0x0B, 0x06, 0x09, 0x2A, 
		0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x05, 0x03, 0x81, 0x81, 0x00, 
		0x1D, 0xB0, 0x1C, 0x88, 0x4D, 0xA2, 0x68, 0x25, 0x08, 0x8F, 0xA3, 0xAC, 
		0xC3, 0x18, 0xD5, 0xBF, 0x56, 0x7C, 0xA1, 0xF2, 0x7C, 0x76, 0x39, 0x8D, 
		0x12, 0x42, 0x17, 0xE6, 0x49, 0x02, 0x39, 0xAE, 0xBB, 0x75, 0x70, 0x4B, 
		0x65, 0xEF, 0x0E, 0x3A, 0xC2, 0x33, 0xD9, 0x94, 0xDF, 0x5F, 0xA6, 0x12, 
		0x64, 0x8F, 0x04, 0x76, 0x2C, 0xAF, 0x92, 0x37, 0x4C, 0xF1, 0x94, 0x99, 
		0x52, 0xFD, 0x61, 0x95, 0x00, 0x2B, 0x9D, 0x0D, 0x35, 0xB9, 0x7C, 0x6A, 
		0x4C, 0xBB, 0x8D, 0x8A, 0x7B, 0x93, 0x37, 0x02, 0xC8, 0x81, 0x0B, 0xBD, 
		0xB9, 0x45, 0x51, 0x03, 0xBA, 0xD3, 0xF4, 0xBD, 0x72, 0x10, 0x05, 0xE9, 
		0xC1, 0x6E, 0xFE, 0xC5, 0x76, 0x2C, 0x6A, 0x6A, 0x16, 0x2F, 0x0C, 0x54, 
		0x44, 0x0D, 0x15, 0xC7, 0xA5, 0x41, 0xB1, 0x05, 0xE8, 0x4B, 0xF3, 0x60, 
		0x92, 0xEB, 0xD4, 0xF7, 0x93, 0xFF, 0x67, 0x4E, 0x31, 0x00 };
	
		static byte[] crlonly = { 
		0x30, 0x82, 0x01, 0x9B, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 
		0x01, 0x07, 0x02, 0xA0, 0x82, 0x01, 0x8C, 0x30, 0x82, 0x01, 0x88, 0x02, 
		0x01, 0x01, 0x31, 0x00, 0x30, 0x0B, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 
		0xF7, 0x0D, 0x01, 0x07, 0x01, 0xA1, 0x82, 0x01, 0x70, 0x30, 0x82, 0x01, 
		0x6C, 0x30, 0x81, 0xD8, 0x02, 0x01, 0x01, 0x30, 0x0B, 0x06, 0x09, 0x2A, 
		0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x05, 0x30, 0x56, 0x31, 0x0B, 
		0x30, 0x09, 0x06, 0x03, 0x55, 0x04, 0x06, 0x13, 0x02, 0x55, 0x53, 0x31, 
		0x18, 0x30, 0x16, 0x06, 0x03, 0x55, 0x04, 0x0A, 0x13, 0x0F, 0x55, 0x2E, 
		0x53, 0x2E, 0x20, 0x47, 0x6F, 0x76, 0x65, 0x72, 0x6E, 0x6D, 0x65, 0x6E, 
		0x74, 0x31, 0x0C, 0x30, 0x0A, 0x06, 0x03, 0x55, 0x04, 0x0B, 0x13, 0x03, 
		0x44, 0x6F, 0x44, 0x31, 0x0D, 0x30, 0x0B, 0x06, 0x03, 0x55, 0x04, 0x0B, 
		0x13, 0x04, 0x4E, 0x61, 0x76, 0x79, 0x31, 0x10, 0x30, 0x0E, 0x06, 0x03, 
		0x55, 0x04, 0x03, 0x13, 0x07, 0x4E, 0x61, 0x76, 0x79, 0x20, 0x43, 0x41, 
		0x17, 0x0D, 0x30, 0x32, 0x31, 0x30, 0x31, 0x31, 0x31, 0x33, 0x31, 0x32, 
		0x35, 0x30, 0x5A, 0x17, 0x0D, 0x30, 0x33, 0x31, 0x30, 0x31, 0x31, 0x31, 
		0x33, 0x31, 0x32, 0x35, 0x30, 0x5A, 0x30, 0x50, 0x30, 0x12, 0x02, 0x01, 
		0x6D, 0x17, 0x0D, 0x30, 0x31, 0x30, 0x34, 0x32, 0x33, 0x32, 0x31, 0x30, 
		0x39, 0x32, 0x37, 0x5A, 0x30, 0x12, 0x02, 0x01, 0x6F, 0x17, 0x0D, 0x30, 
		0x31, 0x30, 0x34, 0x32, 0x33, 0x32, 0x31, 0x30, 0x39, 0x32, 0x37, 0x5A, 
		0x30, 0x12, 0x02, 0x01, 0x50, 0x17, 0x0D, 0x30, 0x30, 0x31, 0x31, 0x33, 
		0x30, 0x32, 0x32, 0x30, 0x38, 0x32, 0x39, 0x5A, 0x30, 0x12, 0x02, 0x01, 
		0x52, 0x17, 0x0D, 0x30, 0x30, 0x31, 0x31, 0x33, 0x30, 0x32, 0x32, 0x30, 
		0x38, 0x32, 0x39, 0x5A, 0x30, 0x0B, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 
		0xF7, 0x0D, 0x01, 0x01, 0x05, 0x03, 0x81, 0x81, 0x00, 0x3A, 0xFA, 0x41, 
		0x76, 0x90, 0x24, 0x6E, 0x59, 0xEE, 0xF3, 0xC4, 0xA2, 0x77, 0xE0, 0xE4, 
		0x70, 0x69, 0x43, 0xA0, 0x8E, 0x42, 0x9F, 0x1F, 0x58, 0x43, 0x1D, 0xF0, 
		0x4F, 0x1D, 0xE8, 0xF3, 0x36, 0x09, 0x07, 0xE5, 0x3A, 0x84, 0xBB, 0x54, 
		0xBB, 0xB6, 0x55, 0x88, 0x76, 0xC2, 0x42, 0x62, 0xC1, 0xE9, 0x54, 0xA2, 
		0x49, 0xEE, 0x98, 0xDD, 0x07, 0x84, 0x90, 0x5F, 0x7E, 0x94, 0x11, 0x64, 
		0x35, 0x2D, 0xBA, 0x5A, 0xC7, 0x19, 0x46, 0xAF, 0x21, 0x3C, 0x3B, 0xB6, 
		0x0E, 0x28, 0x2B, 0x38, 0x9A, 0xA1, 0xB6, 0x7B, 0x6A, 0xC8, 0xA8, 0xBA, 
		0xC7, 0x9E, 0xD1, 0x31, 0x70, 0x5F, 0xD6, 0x15, 0x03, 0xE6, 0x6C, 0x55, 
		0x85, 0x30, 0xA8, 0x45, 0xBB, 0x28, 0xF3, 0xAC, 0x97, 0x5F, 0x86, 0x21, 
		0x77, 0xEF, 0xEC, 0x17, 0x92, 0xC7, 0xD6, 0xCD, 0xE1, 0x2A, 0x2E, 0xE7, 
		0xF3, 0xED, 0x7F, 0x66, 0x86, 0x31, 0x00 };
	
		static byte[] navy = { 
		0x30, 0x82, 0x0B, 0x7F, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 
		0x01, 0x07, 0x02, 0xA0, 0x82, 0x0B, 0x70, 0x30, 0x82, 0x0B, 0x6C, 0x02, 
		0x01, 0x01, 0x31, 0x00, 0x30, 0x0B, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 
		0xF7, 0x0D, 0x01, 0x07, 0x01, 0xA0, 0x82, 0x08, 0xB2, 0x30, 0x82, 0x02, 
		0xAD, 0x30, 0x82, 0x02, 0x18, 0xA0, 0x03, 0x02, 0x01, 0x02, 0x02, 0x01, 
		0x0C, 0x30, 0x0B, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 
		0x01, 0x05, 0x30, 0x51, 0x31, 0x0B, 0x30, 0x09, 0x06, 0x03, 0x55, 0x04, 
		0x06, 0x13, 0x02, 0x55, 0x53, 0x31, 0x18, 0x30, 0x16, 0x06, 0x03, 0x55, 
		0x04, 0x0A, 0x13, 0x0F, 0x55, 0x2E, 0x53, 0x2E, 0x20, 0x47, 0x6F, 0x76, 
		0x65, 0x72, 0x6E, 0x6D, 0x65, 0x6E, 0x74, 0x31, 0x0C, 0x30, 0x0A, 0x06, 
		0x03, 0x55, 0x04, 0x0B, 0x13, 0x03, 0x44, 0x6F, 0x44, 0x31, 0x1A, 0x30, 
		0x18, 0x06, 0x03, 0x55, 0x04, 0x03, 0x13, 0x11, 0x41, 0x72, 0x6D, 0x65, 
		0x64, 0x20, 0x46, 0x6F, 0x72, 0x63, 0x65, 0x73, 0x20, 0x52, 0x6F, 0x6F, 
		0x74, 0x30, 0x1E, 0x17, 0x0D, 0x30, 0x30, 0x31, 0x30, 0x32, 0x35, 0x30, 
		0x30, 0x30, 0x30, 0x30, 0x30, 0x5A, 0x17, 0x0D, 0x30, 0x33, 0x30, 0x31, 
		0x30, 0x31, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x5A, 0x30, 0x50, 0x31, 
		0x0B, 0x30, 0x09, 0x06, 0x03, 0x55, 0x04, 0x06, 0x13, 0x02, 0x55, 0x53, 
		0x31, 0x18, 0x30, 0x16, 0x06, 0x03, 0x55, 0x04, 0x0A, 0x13, 0x0F, 0x55, 
		0x2E, 0x53, 0x2E, 0x20, 0x47, 0x6F, 0x76, 0x65, 0x72, 0x6E, 0x6D, 0x65, 
		0x6E, 0x74, 0x31, 0x0C, 0x30, 0x0A, 0x06, 0x03, 0x55, 0x04, 0x0B, 0x13, 
		0x03, 0x44, 0x6F, 0x44, 0x31, 0x19, 0x30, 0x17, 0x06, 0x03, 0x55, 0x04, 
		0x03, 0x13, 0x10, 0x41, 0x72, 0x6D, 0x65, 0x64, 0x20, 0x46, 0x6F, 0x72, 
		0x63, 0x65, 0x73, 0x20, 0x49, 0x43, 0x41, 0x30, 0x81, 0x9F, 0x30, 0x0D, 
		0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 
		0x00, 0x03, 0x81, 0x8D, 0x00, 0x30, 0x81, 0x89, 0x02, 0x81, 0x81, 0x00, 
		0xD7, 0xA1, 0xB5, 0x18, 0x96, 0x60, 0x18, 0x4F, 0x8A, 0x94, 0x51, 0x47, 
		0x4B, 0x48, 0xC6, 0xE5, 0x78, 0x53, 0x8F, 0x7E, 0x0E, 0x87, 0xD4, 0x05, 
		0x15, 0x50, 0x3F, 0x92, 0x43, 0x4F, 0xBF, 0x43, 0xB5, 0x36, 0xCA, 0x69, 
		0x53, 0x91, 0x1D, 0x68, 0x12, 0x8B, 0x7D, 0x19, 0xAC, 0xA4, 0xA6, 0xDD, 
		0x69, 0x1B, 0x95, 0xF6, 0x68, 0x9D, 0xC1, 0x7D, 0x64, 0xC3, 0x7F, 0xA1, 
		0xCA, 0x2A, 0xC5, 0xE5, 0x0D, 0x5F, 0x00, 0x6A, 0xA7, 0xCF, 0xF0, 0x25, 
		0x86, 0xC4, 0xEA, 0x3B, 0x39, 0x63, 0x00, 0x46, 0x4E, 0xC7, 0xE2, 0xDD, 
		0x47, 0xCF, 0xB3, 0x5E, 0x2F, 0x0B, 0x59, 0x94, 0xE2, 0xCB, 0x04, 0xFA, 
		0x88, 0x30, 0xA3, 0x32, 0xE4, 0x5D, 0x17, 0x09, 0x1A, 0xE1, 0x6D, 0x27, 
		0x03, 0x53, 0x65, 0xC1, 0x1F, 0xE3, 0x73, 0xA4, 0x8D, 0xD8, 0xCB, 0x5A, 
		0x22, 0x07, 0xE4, 0x35, 0x61, 0x8F, 0xD6, 0x57, 0x02, 0x03, 0x01, 0x00, 
		0x01, 0xA3, 0x81, 0x99, 0x30, 0x81, 0x96, 0x30, 0x1F, 0x06, 0x03, 0x55, 
		0x1D, 0x23, 0x04, 0x18, 0x30, 0x16, 0x80, 0x14, 0x3A, 0xCC, 0x94, 0x65, 
		0x0C, 0x85, 0xA9, 0x3C, 0xC1, 0xE0, 0xAF, 0x51, 0x33, 0x2A, 0x14, 0x48, 
		0x8F, 0x9E, 0x91, 0x5B, 0x30, 0x1D, 0x06, 0x03, 0x55, 0x1D, 0x0E, 0x04, 
		0x16, 0x04, 0x14, 0xDA, 0xE9, 0x92, 0x0A, 0xD6, 0x58, 0x28, 0x3A, 0x8B, 
		0x60, 0xCB, 0x20, 0x76, 0x48, 0xB6, 0x5B, 0x0F, 0x10, 0x83, 0x1C, 0x30, 
		0x0B, 0x06, 0x03, 0x55, 0x1D, 0x0F, 0x04, 0x04, 0x03, 0x02, 0x01, 0xEE, 
		0x30, 0x33, 0x06, 0x03, 0x55, 0x1D, 0x20, 0x04, 0x2C, 0x30, 0x2A, 0x30, 
		0x0C, 0x06, 0x0A, 0x60, 0x86, 0x48, 0x01, 0x65, 0x02, 0x01, 0x0C, 0x01, 
		0x01, 0x30, 0x0C, 0x06, 0x0A, 0x60, 0x86, 0x48, 0x01, 0x65, 0x03, 0x02, 
		0x01, 0x30, 0x01, 0x30, 0x0C, 0x06, 0x0A, 0x60, 0x86, 0x48, 0x01, 0x65, 
		0x03, 0x02, 0x01, 0x30, 0x02, 0x30, 0x12, 0x06, 0x03, 0x55, 0x1D, 0x13, 
		0x01, 0x01, 0xFF, 0x04, 0x08, 0x30, 0x06, 0x01, 0x01, 0xFF, 0x02, 0x01, 
		0x01, 0x30, 0x0B, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 
		0x01, 0x05, 0x03, 0x81, 0x81, 0x00, 0x97, 0xD9, 0xE9, 0x1F, 0x79, 0x38, 
		0x6C, 0xF3, 0xD0, 0x94, 0xC0, 0xDF, 0xFC, 0xBB, 0x94, 0xE5, 0x82, 0x78, 
		0xA7, 0x96, 0xC1, 0x92, 0x53, 0x18, 0xED, 0x2B, 0xF0, 0xE1, 0x51, 0x72, 
		0xD0, 0xAF, 0x16, 0x3B, 0xEB, 0xCA, 0x98, 0x4F, 0xE8, 0xD8, 0xA8, 0x88, 
		0x62, 0xBE, 0x0A, 0xA2, 0x38, 0x61, 0x83, 0x65, 0x95, 0x6A, 0x9C, 0x13, 
		0x20, 0xD1, 0x7C, 0x6A, 0xA1, 0x52, 0x5B, 0x7C, 0x49, 0x29, 0xB4, 0x85, 
		0x61, 0xA0, 0x98, 0x2D, 0xDA, 0x43, 0x3E, 0xC4, 0xEF, 0x81, 0xCE, 0x5F, 
		0xF6, 0xAD, 0x69, 0xE3, 0xD6, 0xB5, 0x5E, 0x17, 0x21, 0xFF, 0x1D, 0x64, 
		0x18, 0xA4, 0x61, 0x07, 0x55, 0xA1, 0x93, 0x92, 0x1C, 0x8B, 0xCD, 0x9A, 
		0x8F, 0x66, 0xCB, 0xCB, 0x63, 0x2B, 0x3D, 0xB2, 0x31, 0x4F, 0x3A, 0x5E, 
		0x8B, 0x90, 0xCD, 0x91, 0x70, 0xB7, 0xC1, 0x66, 0xB3, 0x38, 0x5C, 0x83, 
		0xB9, 0x5E, 0x30, 0x82, 0x03, 0x0A, 0x30, 0x82, 0x02, 0x75, 0xA0, 0x03, 
		0x02, 0x01, 0x02, 0x02, 0x01, 0x0D, 0x30, 0x0B, 0x06, 0x09, 0x2A, 0x86, 
		0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x05, 0x30, 0x50, 0x31, 0x0B, 0x30, 
		0x09, 0x06, 0x03, 0x55, 0x04, 0x06, 0x13, 0x02, 0x55, 0x53, 0x31, 0x18, 
		0x30, 0x16, 0x06, 0x03, 0x55, 0x04, 0x0A, 0x13, 0x0F, 0x55, 0x2E, 0x53, 
		0x2E, 0x20, 0x47, 0x6F, 0x76, 0x65, 0x72, 0x6E, 0x6D, 0x65, 0x6E, 0x74, 
		0x31, 0x0C, 0x30, 0x0A, 0x06, 0x03, 0x55, 0x04, 0x0B, 0x13, 0x03, 0x44, 
		0x6F, 0x44, 0x31, 0x19, 0x30, 0x17, 0x06, 0x03, 0x55, 0x04, 0x03, 0x13, 
		0x10, 0x41, 0x72, 0x6D, 0x65, 0x64, 0x20, 0x46, 0x6F, 0x72, 0x63, 0x65, 
		0x73, 0x20, 0x49, 0x43, 0x41, 0x30, 0x1E, 0x17, 0x0D, 0x30, 0x30, 0x31, 
		0x30, 0x32, 0x36, 0x30, 0x30, 0x31, 0x31, 0x32, 0x35, 0x5A, 0x17, 0x0D, 
		0x30, 0x33, 0x30, 0x31, 0x30, 0x31, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 
		0x5A, 0x30, 0x56, 0x31, 0x0B, 0x30, 0x09, 0x06, 0x03, 0x55, 0x04, 0x06, 
		0x13, 0x02, 0x55, 0x53, 0x31, 0x18, 0x30, 0x16, 0x06, 0x03, 0x55, 0x04, 
		0x0A, 0x13, 0x0F, 0x55, 0x2E, 0x53, 0x2E, 0x20, 0x47, 0x6F, 0x76, 0x65, 
		0x72, 0x6E, 0x6D, 0x65, 0x6E, 0x74, 0x31, 0x0C, 0x30, 0x0A, 0x06, 0x03, 
		0x55, 0x04, 0x0B, 0x13, 0x03, 0x44, 0x6F, 0x44, 0x31, 0x0D, 0x30, 0x0B, 
		0x06, 0x03, 0x55, 0x04, 0x0B, 0x13, 0x04, 0x4E, 0x61, 0x76, 0x79, 0x31, 
		0x10, 0x30, 0x0E, 0x06, 0x03, 0x55, 0x04, 0x03, 0x13, 0x07, 0x4E, 0x61, 
		0x76, 0x79, 0x20, 0x43, 0x41, 0x30, 0x81, 0x9F, 0x30, 0x0D, 0x06, 0x09, 
		0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00, 0x03, 
		0x81, 0x8D, 0x00, 0x30, 0x81, 0x89, 0x02, 0x81, 0x81, 0x00, 0x8B, 0xBF, 
		0x07, 0x73, 0x6C, 0x4C, 0xA1, 0x5D, 0xDC, 0xDC, 0x2E, 0x8E, 0x0C, 0xE7, 
		0xBD, 0x8F, 0xC7, 0x1B, 0x31, 0xBE, 0x60, 0x85, 0x5D, 0x75, 0xE5, 0xC9, 
		0xFD, 0xB2, 0x14, 0x62, 0xA8, 0xF4, 0x80, 0x88, 0x7A, 0x14, 0xD7, 0x9B, 
		0x17, 0x5A, 0x79, 0x0E, 0x70, 0xB8, 0xB6, 0x58, 0xC1, 0xBE, 0xEB, 0xB1, 
		0xE5, 0x43, 0x2B, 0x98, 0x63, 0xA2, 0xEB, 0xF3, 0x28, 0x0E, 0x68, 0x95, 
		0x2F, 0xC5, 0x72, 0x56, 0xBF, 0x3B, 0xCF, 0xAF, 0x47, 0xC7, 0x80, 0xCA, 
		0x52, 0x3C, 0x26, 0x1D, 0xAF, 0x0A, 0x39, 0x0F, 0x0B, 0xE4, 0xA7, 0x24, 
		0x97, 0x23, 0x42, 0x8A, 0xAA, 0x2F, 0x2F, 0xE2, 0x16, 0xBE, 0x5F, 0xE2, 
		0x7D, 0xCB, 0xD4, 0xDE, 0xD4, 0x36, 0xA2, 0x53, 0xAA, 0xE2, 0xF8, 0xD8, 
		0x46, 0xEA, 0x6E, 0xF0, 0xD7, 0x66, 0xA5, 0x8E, 0x08, 0x66, 0x5E, 0x94, 
		0x41, 0x27, 0x11, 0xE4, 0xFE, 0xA5, 0x02, 0x03, 0x01, 0x00, 0x01, 0xA3, 
		0x81, 0xF1, 0x30, 0x81, 0xEE, 0x30, 0x1F, 0x06, 0x03, 0x55, 0x1D, 0x23, 
		0x04, 0x18, 0x30, 0x16, 0x80, 0x14, 0xDA, 0xE9, 0x92, 0x0A, 0xD6, 0x58, 
		0x28, 0x3A, 0x8B, 0x60, 0xCB, 0x20, 0x76, 0x48, 0xB6, 0x5B, 0x0F, 0x10, 
		0x83, 0x1C, 0x30, 0x1D, 0x06, 0x03, 0x55, 0x1D, 0x0E, 0x04, 0x16, 0x04, 
		0x14, 0xFB, 0x96, 0xF0, 0x10, 0xC4, 0x37, 0x55, 0xF0, 0xCE, 0xB5, 0xA6, 
		0xE2, 0xF1, 0x19, 0xFF, 0x99, 0x1A, 0xAE, 0x6E, 0x58, 0x30, 0x0B, 0x06, 
		0x03, 0x55, 0x1D, 0x0F, 0x04, 0x04, 0x03, 0x02, 0x01, 0xEE, 0x30, 0x33, 
		0x06, 0x03, 0x55, 0x1D, 0x20, 0x04, 0x2C, 0x30, 0x2A, 0x30, 0x0C, 0x06, 
		0x0A, 0x60, 0x86, 0x48, 0x01, 0x65, 0x02, 0x01, 0x0C, 0x01, 0x01, 0x30, 
		0x0C, 0x06, 0x0A, 0x60, 0x86, 0x48, 0x01, 0x65, 0x03, 0x02, 0x01, 0x30, 
		0x01, 0x30, 0x0C, 0x06, 0x0A, 0x60, 0x86, 0x48, 0x01, 0x65, 0x03, 0x02, 
		0x01, 0x30, 0x02, 0x30, 0x12, 0x06, 0x03, 0x55, 0x1D, 0x13, 0x01, 0x01, 
		0xFF, 0x04, 0x08, 0x30, 0x06, 0x01, 0x01, 0xFF, 0x02, 0x01, 0x00, 0x30, 
		0x56, 0x06, 0x04, 0x55, 0x1D, 0x1E, 0x01, 0x04, 0x4E, 0x30, 0x4C, 0xA0, 
		0x4A, 0x30, 0x48, 0xA4, 0x46, 0x30, 0x44, 0x31, 0x0B, 0x30, 0x09, 0x06, 
		0x03, 0x55, 0x04, 0x06, 0x13, 0x02, 0x55, 0x53, 0x31, 0x18, 0x30, 0x16, 
		0x06, 0x03, 0x55, 0x04, 0x0A, 0x13, 0x0F, 0x55, 0x2E, 0x53, 0x2E, 0x20, 
		0x47, 0x6F, 0x76, 0x65, 0x72, 0x6E, 0x6D, 0x65, 0x6E, 0x74, 0x31, 0x0C, 
		0x30, 0x0A, 0x06, 0x03, 0x55, 0x04, 0x0B, 0x13, 0x03, 0x44, 0x6F, 0x44, 
		0x31, 0x0D, 0x30, 0x0B, 0x06, 0x03, 0x55, 0x04, 0x0B, 0x13, 0x04, 0x4E, 
		0x61, 0x76, 0x79, 0x30, 0x0B, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 
		0x0D, 0x01, 0x01, 0x05, 0x03, 0x81, 0x81, 0x00, 0x8C, 0xD1, 0xBD, 0xAD, 
		0xF2, 0xE9, 0xAC, 0xAC, 0x14, 0x22, 0x7B, 0x68, 0x59, 0x09, 0x40, 0xA3, 
		0x7D, 0x14, 0x18, 0x38, 0x03, 0xB9, 0xF2, 0x74, 0xD6, 0x8F, 0x75, 0xD4, 
		0xBB, 0xA4, 0x53, 0xA7, 0x4A, 0x7A, 0x7E, 0x77, 0x3D, 0x0E, 0xA6, 0xB8, 
		0xDE, 0x10, 0x3C, 0x14, 0xFB, 0xDE, 0x15, 0xC5, 0x81, 0xE1, 0x18, 0x31, 
		0xB8, 0xB8, 0xFE, 0x5C, 0x60, 0xC5, 0x3A, 0x80, 0xAB, 0x76, 0x22, 0xF6, 
		0xDF, 0x4A, 0xBD, 0x07, 0xE8, 0x76, 0x45, 0xF9, 0xF3, 0xF7, 0x9F, 0x3B, 
		0x40, 0xD3, 0xAE, 0xAE, 0x64, 0x1A, 0xB0, 0x2C, 0x62, 0xB7, 0xBD, 0xBD, 
		0x6F, 0x80, 0x22, 0x59, 0x99, 0x1B, 0x0E, 0xA6, 0x8E, 0xEB, 0x92, 0xF8, 
		0xCA, 0xA9, 0xD9, 0x55, 0xDD, 0x60, 0x65, 0xA8, 0x5C, 0xAA, 0x56, 0xE0, 
		0x36, 0xD1, 0xC0, 0x09, 0x4F, 0xBA, 0x3B, 0xD7, 0xEC, 0x6F, 0xA2, 0xE3, 
		0x02, 0xBF, 0xFB, 0x45, 0x30, 0x82, 0x02, 0xEF, 0x30, 0x82, 0x02, 0x5A, 
		0xA0, 0x03, 0x02, 0x01, 0x02, 0x02, 0x01, 0x6C, 0x30, 0x0B, 0x06, 0x09, 
		0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x05, 0x30, 0x56, 0x31, 
		0x0B, 0x30, 0x09, 0x06, 0x03, 0x55, 0x04, 0x06, 0x13, 0x02, 0x55, 0x53, 
		0x31, 0x18, 0x30, 0x16, 0x06, 0x03, 0x55, 0x04, 0x0A, 0x13, 0x0F, 0x55, 
		0x2E, 0x53, 0x2E, 0x20, 0x47, 0x6F, 0x76, 0x65, 0x72, 0x6E, 0x6D, 0x65, 
		0x6E, 0x74, 0x31, 0x0C, 0x30, 0x0A, 0x06, 0x03, 0x55, 0x04, 0x0B, 0x13, 
		0x03, 0x44, 0x6F, 0x44, 0x31, 0x0D, 0x30, 0x0B, 0x06, 0x03, 0x55, 0x04, 
		0x0B, 0x13, 0x04, 0x4E, 0x61, 0x76, 0x79, 0x31, 0x10, 0x30, 0x0E, 0x06, 
		0x03, 0x55, 0x04, 0x03, 0x13, 0x07, 0x4E, 0x61, 0x76, 0x79, 0x20, 0x43, 
		0x41, 0x30, 0x1E, 0x17, 0x0D, 0x30, 0x32, 0x31, 0x30, 0x31, 0x31, 0x31, 
		0x33, 0x31, 0x32, 0x35, 0x30, 0x5A, 0x17, 0x0D, 0x30, 0x34, 0x31, 0x30, 
		0x31, 0x30, 0x31, 0x33, 0x31, 0x32, 0x35, 0x30, 0x5A, 0x30, 0x81, 0x8B, 
		0x31, 0x0B, 0x30, 0x09, 0x06, 0x03, 0x55, 0x04, 0x06, 0x13, 0x02, 0x55, 
		0x53, 0x31, 0x18, 0x30, 0x16, 0x06, 0x03, 0x55, 0x04, 0x0A, 0x13, 0x0F, 
		0x55, 0x2E, 0x53, 0x2E, 0x20, 0x47, 0x6F, 0x76, 0x65, 0x72, 0x6E, 0x6D, 
		0x65, 0x6E, 0x74, 0x31, 0x0C, 0x30, 0x0A, 0x06, 0x03, 0x55, 0x04, 0x0B, 
		0x13, 0x03, 0x44, 0x6F, 0x44, 0x31, 0x0D, 0x30, 0x0B, 0x06, 0x03, 0x55, 
		0x04, 0x0B, 0x13, 0x04, 0x4E, 0x61, 0x76, 0x79, 0x31, 0x12, 0x30, 0x10, 
		0x06, 0x03, 0x55, 0x04, 0x0B, 0x13, 0x09, 0x6C, 0x6F, 0x63, 0x61, 0x74, 
		0x69, 0x6F, 0x6E, 0x73, 0x31, 0x1B, 0x30, 0x19, 0x06, 0x03, 0x55, 0x04, 
		0x07, 0x13, 0x12, 0x41, 0x6E, 0x6E, 0x61, 0x70, 0x6F, 0x6C, 0x69, 0x73, 
		0x20, 0x4A, 0x75, 0x6E, 0x63, 0x74, 0x69, 0x6F, 0x6E, 0x31, 0x14, 0x30, 
		0x12, 0x06, 0x03, 0x55, 0x04, 0x03, 0x13, 0x0B, 0x4E, 0x61, 0x76, 0x79, 
		0x20, 0x55, 0x73, 0x65, 0x72, 0x20, 0x31, 0x30, 0x81, 0x9F, 0x30, 0x0D, 
		0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 
		0x00, 0x03, 0x81, 0x8D, 0x00, 0x30, 0x81, 0x89, 0x02, 0x81, 0x81, 0x00, 
		0xB7, 0x7E, 0x94, 0x5F, 0xE8, 0x2A, 0xE7, 0xAD, 0x82, 0x16, 0x2C, 0x3D, 
		0x2F, 0x5E, 0x88, 0x67, 0xF0, 0x23, 0x26, 0x15, 0x34, 0x04, 0x1F, 0x63, 
		0x8B, 0xFE, 0xFB, 0xBB, 0x0D, 0xC0, 0x7E, 0xF0, 0x46, 0x82, 0x09, 0xA2, 
		0x91, 0xE0, 0xEA, 0xEF, 0xD0, 0x43, 0xCB, 0x30, 0x45, 0xAC, 0x7C, 0xAC, 
		0xFC, 0xBE, 0x54, 0x79, 0x77, 0xA9, 0x6A, 0x45, 0xF5, 0xBF, 0xE5, 0xEF, 
		0x97, 0x11, 0x63, 0xC2, 0xF7, 0x3C, 0x73, 0x6D, 0xBA, 0x8D, 0xFE, 0xAE, 
		0x28, 0x4A, 0x29, 0xE4, 0xA2, 0x59, 0x0C, 0x8F, 0x1A, 0x57, 0x86, 0xF2, 
		0x42, 0xF7, 0x35, 0x0B, 0xC3, 0xA5, 0x31, 0xD8, 0x19, 0xE2, 0x97, 0x7A, 
		0xA1, 0xF4, 0xE5, 0xDB, 0xCA, 0xF5, 0x54, 0x39, 0x1D, 0x0E, 0xDF, 0x78, 
		0x73, 0xBF, 0x86, 0x97, 0x40, 0xAA, 0x06, 0x8E, 0x8B, 0x6B, 0x0C, 0x06, 
		0x98, 0xD7, 0xD2, 0x1D, 0x45, 0xAA, 0x7F, 0xA5, 0x02, 0x03, 0x01, 0x00, 
		0x01, 0xA3, 0x81, 0x9A, 0x30, 0x81, 0x97, 0x30, 0x1F, 0x06, 0x03, 0x55, 
		0x1D, 0x23, 0x04, 0x18, 0x30, 0x16, 0x80, 0x14, 0xFB, 0x96, 0xF0, 0x10, 
		0xC4, 0x37, 0x55, 0xF0, 0xCE, 0xB5, 0xA6, 0xE2, 0xF1, 0x19, 0xFF, 0x99, 
		0x1A, 0xAE, 0x6E, 0x58, 0x30, 0x1D, 0x06, 0x03, 0x55, 0x1D, 0x0E, 0x04, 
		0x16, 0x04, 0x14, 0x02, 0x48, 0x78, 0xB9, 0xCC, 0x01, 0x51, 0x31, 0x74, 
		0x7F, 0x39, 0x2A, 0x37, 0xC2, 0x44, 0x93, 0x7E, 0x98, 0x69, 0x80, 0x30, 
		0x0B, 0x06, 0x03, 0x55, 0x1D, 0x0F, 0x04, 0x04, 0x03, 0x02, 0x04, 0xF0, 
		0x30, 0x17, 0x06, 0x03, 0x55, 0x1D, 0x20, 0x04, 0x10, 0x30, 0x0E, 0x30, 
		0x0C, 0x06, 0x0A, 0x60, 0x86, 0x48, 0x01, 0x65, 0x03, 0x02, 0x01, 0x30, 
		0x01, 0x30, 0x2F, 0x06, 0x03, 0x55, 0x1D, 0x11, 0x04, 0x28, 0x30, 0x26, 
		0x81, 0x24, 0x4E, 0x61, 0x76, 0x79, 0x31, 0x40, 0x77, 0x61, 0x72, 0x72, 
		0x65, 0x6E, 0x74, 0x6F, 0x6E, 0x2E, 0x61, 0x74, 0x6C, 0x2E, 0x67, 0x65, 
		0x74, 0x72, 0x6F, 0x6E, 0x69, 0x63, 0x73, 0x67, 0x6F, 0x76, 0x2E, 0x63, 
		0x6F, 0x6D, 0x30, 0x0B, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 
		0x01, 0x01, 0x05, 0x03, 0x81, 0x81, 0x00, 0x1D, 0xB0, 0x1C, 0x88, 0x4D, 
		0xA2, 0x68, 0x25, 0x08, 0x8F, 0xA3, 0xAC, 0xC3, 0x18, 0xD5, 0xBF, 0x56, 
		0x7C, 0xA1, 0xF2, 0x7C, 0x76, 0x39, 0x8D, 0x12, 0x42, 0x17, 0xE6, 0x49, 
		0x02, 0x39, 0xAE, 0xBB, 0x75, 0x70, 0x4B, 0x65, 0xEF, 0x0E, 0x3A, 0xC2, 
		0x33, 0xD9, 0x94, 0xDF, 0x5F, 0xA6, 0x12, 0x64, 0x8F, 0x04, 0x76, 0x2C, 
		0xAF, 0x92, 0x37, 0x4C, 0xF1, 0x94, 0x99, 0x52, 0xFD, 0x61, 0x95, 0x00, 
		0x2B, 0x9D, 0x0D, 0x35, 0xB9, 0x7C, 0x6A, 0x4C, 0xBB, 0x8D, 0x8A, 0x7B, 
		0x93, 0x37, 0x02, 0xC8, 0x81, 0x0B, 0xBD, 0xB9, 0x45, 0x51, 0x03, 0xBA, 
		0xD3, 0xF4, 0xBD, 0x72, 0x10, 0x05, 0xE9, 0xC1, 0x6E, 0xFE, 0xC5, 0x76, 
		0x2C, 0x6A, 0x6A, 0x16, 0x2F, 0x0C, 0x54, 0x44, 0x0D, 0x15, 0xC7, 0xA5, 
		0x41, 0xB1, 0x05, 0xE8, 0x4B, 0xF3, 0x60, 0x92, 0xEB, 0xD4, 0xF7, 0x93, 
		0xFF, 0x67, 0x4E, 0xA1, 0x82, 0x02, 0x9E, 0x30, 0x82, 0x01, 0x2A, 0x30, 
		0x81, 0x96, 0x02, 0x01, 0x01, 0x30, 0x0B, 0x06, 0x09, 0x2A, 0x86, 0x48, 
		0x86, 0xF7, 0x0D, 0x01, 0x01, 0x05, 0x30, 0x50, 0x31, 0x0B, 0x30, 0x09, 
		0x06, 0x03, 0x55, 0x04, 0x06, 0x13, 0x02, 0x55, 0x53, 0x31, 0x18, 0x30, 
		0x16, 0x06, 0x03, 0x55, 0x04, 0x0A, 0x13, 0x0F, 0x55, 0x2E, 0x53, 0x2E, 
		0x20, 0x47, 0x6F, 0x76, 0x65, 0x72, 0x6E, 0x6D, 0x65, 0x6E, 0x74, 0x31, 
		0x0C, 0x30, 0x0A, 0x06, 0x03, 0x55, 0x04, 0x0B, 0x13, 0x03, 0x44, 0x6F, 
		0x44, 0x31, 0x19, 0x30, 0x17, 0x06, 0x03, 0x55, 0x04, 0x03, 0x13, 0x10, 
		0x41, 0x72, 0x6D, 0x65, 0x64, 0x20, 0x46, 0x6F, 0x72, 0x63, 0x65, 0x73, 
		0x20, 0x49, 0x43, 0x41, 0x17, 0x0D, 0x30, 0x32, 0x31, 0x30, 0x31, 0x31, 
		0x31, 0x33, 0x31, 0x32, 0x35, 0x30, 0x5A, 0x17, 0x0D, 0x30, 0x33, 0x31, 
		0x30, 0x31, 0x31, 0x31, 0x33, 0x31, 0x32, 0x35, 0x30, 0x5A, 0x30, 0x14, 
		0x30, 0x12, 0x02, 0x01, 0x09, 0x17, 0x0D, 0x30, 0x30, 0x31, 0x31, 0x33, 
		0x30, 0x32, 0x31, 0x31, 0x36, 0x35, 0x31, 0x5A, 0x30, 0x0B, 0x06, 0x09, 
		0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x05, 0x03, 0x81, 0x81, 
		0x00, 0x7B, 0xC3, 0x89, 0xC4, 0x94, 0xEA, 0x2A, 0x44, 0x61, 0x96, 0xC9, 
		0x82, 0x05, 0x67, 0xE4, 0x8F, 0xBC, 0xE8, 0x8A, 0x7B, 0xA6, 0xA8, 0xD6, 
		0x82, 0x9A, 0x2B, 0x3D, 0x56, 0x15, 0xEA, 0x3B, 0x58, 0xAC, 0xC6, 0xED, 
		0xCB, 0x67, 0x0B, 0x1F, 0x37, 0x21, 0xF2, 0x50, 0xF3, 0x41, 0x40, 0x09, 
		0x9F, 0xE3, 0xF5, 0xF5, 0x20, 0x0F, 0xEA, 0xC7, 0xA4, 0xD1, 0xBA, 0xAE, 
		0xB2, 0x92, 0x9E, 0x5E, 0x3D, 0xFE, 0xE5, 0xD5, 0x79, 0xAD, 0xA7, 0x29, 
		0x63, 0xFC, 0x39, 0x03, 0xC2, 0x16, 0x95, 0x2C, 0xB0, 0x40, 0xED, 0x2E, 
		0x09, 0xF7, 0x1C, 0x94, 0x6A, 0xB6, 0x92, 0x7D, 0x9C, 0x35, 0x83, 0xEE, 
		0x0D, 0x98, 0xD0, 0xC3, 0x2E, 0xD5, 0x0C, 0xE4, 0xCE, 0x6D, 0x36, 0xC0, 
		0x27, 0x16, 0x3A, 0x34, 0x33, 0x54, 0x96, 0x7D, 0xB4, 0x91, 0x03, 0x39, 
		0x9E, 0x6B, 0x1B, 0x57, 0x8B, 0x9F, 0x4F, 0x10, 0xB2, 0x30, 0x82, 0x01, 
		0x6C, 0x30, 0x81, 0xD8, 0x02, 0x01, 0x01, 0x30, 0x0B, 0x06, 0x09, 0x2A, 
		0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x05, 0x30, 0x56, 0x31, 0x0B, 
		0x30, 0x09, 0x06, 0x03, 0x55, 0x04, 0x06, 0x13, 0x02, 0x55, 0x53, 0x31, 
		0x18, 0x30, 0x16, 0x06, 0x03, 0x55, 0x04, 0x0A, 0x13, 0x0F, 0x55, 0x2E, 
		0x53, 0x2E, 0x20, 0x47, 0x6F, 0x76, 0x65, 0x72, 0x6E, 0x6D, 0x65, 0x6E, 
		0x74, 0x31, 0x0C, 0x30, 0x0A, 0x06, 0x03, 0x55, 0x04, 0x0B, 0x13, 0x03, 
		0x44, 0x6F, 0x44, 0x31, 0x0D, 0x30, 0x0B, 0x06, 0x03, 0x55, 0x04, 0x0B, 
		0x13, 0x04, 0x4E, 0x61, 0x76, 0x79, 0x31, 0x10, 0x30, 0x0E, 0x06, 0x03, 
		0x55, 0x04, 0x03, 0x13, 0x07, 0x4E, 0x61, 0x76, 0x79, 0x20, 0x43, 0x41, 
		0x17, 0x0D, 0x30, 0x32, 0x31, 0x30, 0x31, 0x31, 0x31, 0x33, 0x31, 0x32, 
		0x35, 0x30, 0x5A, 0x17, 0x0D, 0x30, 0x33, 0x31, 0x30, 0x31, 0x31, 0x31, 
		0x33, 0x31, 0x32, 0x35, 0x30, 0x5A, 0x30, 0x50, 0x30, 0x12, 0x02, 0x01, 
		0x6D, 0x17, 0x0D, 0x30, 0x31, 0x30, 0x34, 0x32, 0x33, 0x32, 0x31, 0x30, 
		0x39, 0x32, 0x37, 0x5A, 0x30, 0x12, 0x02, 0x01, 0x6F, 0x17, 0x0D, 0x30, 
		0x31, 0x30, 0x34, 0x32, 0x33, 0x32, 0x31, 0x30, 0x39, 0x32, 0x37, 0x5A, 
		0x30, 0x12, 0x02, 0x01, 0x50, 0x17, 0x0D, 0x30, 0x30, 0x31, 0x31, 0x33, 
		0x30, 0x32, 0x32, 0x30, 0x38, 0x32, 0x39, 0x5A, 0x30, 0x12, 0x02, 0x01, 
		0x52, 0x17, 0x0D, 0x30, 0x30, 0x31, 0x31, 0x33, 0x30, 0x32, 0x32, 0x30, 
		0x38, 0x32, 0x39, 0x5A, 0x30, 0x0B, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 
		0xF7, 0x0D, 0x01, 0x01, 0x05, 0x03, 0x81, 0x81, 0x00, 0x3A, 0xFA, 0x41, 
		0x76, 0x90, 0x24, 0x6E, 0x59, 0xEE, 0xF3, 0xC4, 0xA2, 0x77, 0xE0, 0xE4, 
		0x70, 0x69, 0x43, 0xA0, 0x8E, 0x42, 0x9F, 0x1F, 0x58, 0x43, 0x1D, 0xF0, 
		0x4F, 0x1D, 0xE8, 0xF3, 0x36, 0x09, 0x07, 0xE5, 0x3A, 0x84, 0xBB, 0x54, 
		0xBB, 0xB6, 0x55, 0x88, 0x76, 0xC2, 0x42, 0x62, 0xC1, 0xE9, 0x54, 0xA2, 
		0x49, 0xEE, 0x98, 0xDD, 0x07, 0x84, 0x90, 0x5F, 0x7E, 0x94, 0x11, 0x64, 
		0x35, 0x2D, 0xBA, 0x5A, 0xC7, 0x19, 0x46, 0xAF, 0x21, 0x3C, 0x3B, 0xB6, 
		0x0E, 0x28, 0x2B, 0x38, 0x9A, 0xA1, 0xB6, 0x7B, 0x6A, 0xC8, 0xA8, 0xBA, 
		0xC7, 0x9E, 0xD1, 0x31, 0x70, 0x5F, 0xD6, 0x15, 0x03, 0xE6, 0x6C, 0x55, 
		0x85, 0x30, 0xA8, 0x45, 0xBB, 0x28, 0xF3, 0xAC, 0x97, 0x5F, 0x86, 0x21, 
		0x77, 0xEF, 0xEC, 0x17, 0x92, 0xC7, 0xD6, 0xCD, 0xE1, 0x2A, 0x2E, 0xE7, 
		0xF3, 0xED, 0x7F, 0x66, 0x86, 0x31, 0x00 };
	
		private const string testfile = "test.spc";
	
		[TearDown]
		public void TearDown () 
		{
			File.Delete (testfile);
		}
	
		private void WriteBuffer (byte[] buffer, bool base64, bool unicode, bool pem) 
		{
			byte[] data = buffer;
			if (base64) {
				string s = Convert.ToBase64String (buffer);
				if (unicode) {
					data = Encoding.Unicode.GetBytes (s);
				} else if (pem) {
					string b64pem = "-----BEGIN PKCS7-----\n" + s + "\n-----END PKCS7-----";
					data = Encoding.ASCII.GetBytes (b64pem);
			using (FileStream fs = File.Create ("bad.pem")) {
				fs.Write (data, 0, data.Length);
			}
				} else {
					data = Encoding.ASCII.GetBytes (s);
				}
			}

			using (FileStream fs = File.Create (testfile)) {
				fs.Write (data, 0, data.Length);
			}
		}
	
		[Test]
		public void ReadCertificateOnly_Binary () 
		{
			WriteBuffer (certonly, false, false, false);
			SoftwarePublisherCertificate spc = SoftwarePublisherCertificate.CreateFromFile (testfile);
			Assert.AreEqual (1, spc.Certificates.Count, "certonly.Certificates");
			Assert.AreEqual (0, spc.Crls.Count, "certonly.Crl");
		}

		[Test]
		public void ReadCertificateOnly_Base64 () 
		{
			WriteBuffer (certonly, true, false, false);
			SoftwarePublisherCertificate spc = SoftwarePublisherCertificate.CreateFromFile (testfile);
			Assert.AreEqual (1, spc.Certificates.Count, "certonly.Certificates");
			Assert.AreEqual (0, spc.Crls.Count, "certonly.Crl");
		}

		[Test]
		public void ReadCertificateOnly_Base64Unicode () 
		{
			WriteBuffer (certonly, true, true, false);
			SoftwarePublisherCertificate spc = SoftwarePublisherCertificate.CreateFromFile (testfile);
			Assert.AreEqual (1, spc.Certificates.Count, "certonly.Certificates");
			Assert.AreEqual (0, spc.Crls.Count, "certonly.Crl");
		}

		[Test]
		public void ReadCertificateOnly_PEM () 
		{
			WriteBuffer (certonly, true, false, true);
			SoftwarePublisherCertificate spc = SoftwarePublisherCertificate.CreateFromFile (testfile);
			Assert.AreEqual (1, spc.Certificates.Count, "certonly.Certificates");
			Assert.AreEqual (0, spc.Crls.Count, "certonly.Crl");
		}
	
		[Test]
		public void CompareCertificateOnly () 
		{
			WriteBuffer (certonly, false, false, false);
			SoftwarePublisherCertificate spc = SoftwarePublisherCertificate.CreateFromFile (testfile);
			SoftwarePublisherCertificate newspc = new SoftwarePublisherCertificate ();
			newspc.Certificates.Add (spc.Certificates [0]);
			byte[] newcertonly = newspc.GetBytes ();
			Assert.AreEqual (certonly, newcertonly, "certonly.compare");
	
			SoftwarePublisherCertificate newerspc = new SoftwarePublisherCertificate (newcertonly);
			Assert.AreEqual (1, newerspc.Certificates.Count, "certonly.Certificates");
			Assert.AreEqual (0, newerspc.Crls.Count, "certonly.Crl");
		}
	
		[Test]
		public void ReadCRLOnly () 
		{
			WriteBuffer (crlonly, false, false, false);
			SoftwarePublisherCertificate spc = SoftwarePublisherCertificate.CreateFromFile (testfile);
			Assert.AreEqual (0, spc.Certificates.Count, "crlonly.Certificates");
			Assert.AreEqual (1, spc.Crls.Count, "crlonly.Crl");
		}
	
		[Test]
		public void CompareCRLOnly () 
		{
			WriteBuffer (crlonly, false, false, false);
			SoftwarePublisherCertificate spc = SoftwarePublisherCertificate.CreateFromFile (testfile);
			SoftwarePublisherCertificate newspc = new SoftwarePublisherCertificate ();
			newspc.Crls.Add (spc.Crls [0]);
			byte[] newcrlonly = newspc.GetBytes ();
			Assert.AreEqual (crlonly, newcrlonly, "crlonly.compare");
	
			SoftwarePublisherCertificate newerspc = new SoftwarePublisherCertificate (newcrlonly);
			Assert.AreEqual (0, newerspc.Certificates.Count, "crlonly.Certificates");
			Assert.AreEqual (1, newerspc.Crls.Count, "crlonly.Crl");
		}
	
		[Test]
		public void ReadNavy () 
		{
			WriteBuffer (navy, false, false, false);
			SoftwarePublisherCertificate spc = SoftwarePublisherCertificate.CreateFromFile (testfile);
			Assert.AreEqual (3, spc.Certificates.Count, "navy.Certificates");
			Assert.AreEqual (2, spc.Crls.Count, "navy.Crl");
		}
	
		[Test]
		public void CompareReadNavy () 
		{
			WriteBuffer (navy, false, false, false);
			SoftwarePublisherCertificate spc = SoftwarePublisherCertificate.CreateFromFile (testfile);
			SoftwarePublisherCertificate newspc = new SoftwarePublisherCertificate ();
			foreach (MSX.X509Certificate x in spc.Certificates)
				newspc.Certificates.Add (x);
			foreach (byte[] crl in spc.Crls)
				newspc.Crls.Add (crl);
			byte[] newnavy = newspc.GetBytes ();
			Assert.AreEqual (navy, newnavy, "navy.compare");
	
			SoftwarePublisherCertificate newerspc = new SoftwarePublisherCertificate (newnavy);
			Assert.AreEqual (3, newerspc.Certificates.Count, "navy.Certificates");
			Assert.AreEqual (2, newerspc.Crls.Count, "navy.Crl");
		}
		
		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void Constructor_Null () 
		{
			SoftwarePublisherCertificate spc = new SoftwarePublisherCertificate (null);
		}

		[Test]
		[ExpectedException (typeof (ArgumentException))]
		public void Constructor_BadOid () 
		{
			byte[] bad = (byte[]) certonly.Clone ();
			bad [9] -= 1;
			SoftwarePublisherCertificate spc = new SoftwarePublisherCertificate (bad);
		}
		
		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void CreateFromFile_Null () 
		{
			SoftwarePublisherCertificate spc = SoftwarePublisherCertificate.CreateFromFile (null);
		}
	}
}
