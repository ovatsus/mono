// 
// System.Web.Services.Description.ServiceDescription.cs
//
// Author:
//   Tim Coleman (tim@timcoleman.com)
//   Lluis Sanchez Gual (lluis@ximian.com)
//
// Copyright (C) Tim Coleman, 2002
//

using System.IO;
using System.Collections;
using System.Reflection;
using System.Web.Services;
using System.Web.Services.Configuration;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Web.Services.Description {
	[XmlFormatExtensionPoint ("Extensions")]
	[XmlRoot ("definitions", Namespace = "http://schemas.xmlsoap.org/wsdl/")]
	public sealed class ServiceDescription : DocumentableItem {

		#region Fields

		public const string Namespace = "http://schemas.xmlsoap.org/wsdl/";

		BindingCollection bindings;
		ServiceDescriptionFormatExtensionCollection extensions;
		ImportCollection imports;
		MessageCollection messages;
		string name;
		PortTypeCollection portTypes;
		string retrievalUrl;
		ServiceDescriptionCollection serviceDescriptions;
		ServiceCollection services;
		string targetNamespace;
		Types types;
		static ServiceDescriptionSerializer serializer;

		#endregion // Fields

		#region Constructors

		static ServiceDescription ()
		{
			serializer = new ServiceDescriptionSerializer ();
		}

		[MonoTODO ("Move namespaces to subtype, use ServiceDescriptionSerializer")]	
		public ServiceDescription ()
		{
			bindings = new BindingCollection (this);
			extensions = new ServiceDescriptionFormatExtensionCollection (this);
			imports = new ImportCollection (this);
			messages = new MessageCollection (this);
			name = String.Empty;		
			portTypes = new PortTypeCollection (this);

			serviceDescriptions = null;
			services = new ServiceCollection (this);
			targetNamespace = String.Empty;
			types = null;
		}
		
		#endregion // Constructors

		#region Properties

		[XmlElement ("import")]
		public ImportCollection Imports {
			get { return imports; }
		}

		[XmlElement ("types")]
		public Types Types {
			get { return types; }
			set { types = value; }
		}

		[XmlElement ("message")]
		public MessageCollection Messages {
			get { return messages; }
		}

		[XmlElement ("portType")]	
		public PortTypeCollection PortTypes {
			get { return portTypes; }
		}
	
		[XmlElement ("binding")]
		public BindingCollection Bindings {
			get { return bindings; }
		}

		[XmlIgnore]
		public ServiceDescriptionFormatExtensionCollection Extensions { 	
			get { return extensions; }
		}

		[XmlAttribute ("name", DataType = "NMTOKEN")]	
		public string Name {
			get { return name; }
			set { name = value; }
		}

		[XmlIgnore]	
		public string RetrievalUrl {
			get { return retrievalUrl; }
			set { retrievalUrl = value; }
		}
	
		[XmlIgnore]	
		public static XmlSerializer Serializer {
			get { return serializer; }
		}

		[XmlIgnore]
		public ServiceDescriptionCollection ServiceDescriptions {
			get { 
				if (serviceDescriptions == null) 
					throw new NullReferenceException ();
				return serviceDescriptions; 
			}
		}

		[XmlElement ("service")]
		public ServiceCollection Services {
			get { return services; }
		}

		[XmlAttribute ("targetNamespace")]
		public string TargetNamespace {
			get { return targetNamespace; }
			set { targetNamespace = value; }
		}

		#endregion // Properties

		#region Methods

		public static bool CanRead (XmlReader reader)
		{
			return serializer.CanDeserialize (reader);
		}

		public static ServiceDescription Read (Stream stream)
		{
			return (ServiceDescription) serializer.Deserialize (stream);
		}

		public static ServiceDescription Read (string fileName)
		{
			return Read (new FileStream (fileName, FileMode.Open));
		}

		public static ServiceDescription Read (TextReader textReader)
		{
			return (ServiceDescription) serializer.Deserialize (textReader);
		}

		public static ServiceDescription Read (XmlReader reader)
		{
			return (ServiceDescription) serializer.Deserialize (reader);
		}

		public void Write (Stream stream)
		{
			serializer.Serialize (stream, this, GetNamespaceList ());
		}

		public void Write (string fileName)
		{
			Write (new FileStream (fileName, FileMode.Create));
		}

		public void Write (TextWriter writer)
		{
			serializer.Serialize (writer, this, GetNamespaceList ());
		}

		public void Write (XmlWriter writer)
		{
			serializer.Serialize (writer, this, GetNamespaceList ());
		}

		internal void SetParent (ServiceDescriptionCollection serviceDescriptions)
		{
			this.serviceDescriptions = serviceDescriptions; 
		}
		
		XmlSerializerNamespaces GetNamespaceList ()
		{
			XmlSerializerNamespaces ns;
			ns = new XmlSerializerNamespaces ();
			ns.Add ("soap", SoapBinding.Namespace);
			ns.Add ("s", XmlSchema.Namespace);
			ns.Add ("http", HttpBinding.Namespace);
			ns.Add ("mime", MimeContentBinding.Namespace);
			ns.Add ("tm", MimeTextBinding.Namespace);
			ns.Add ("s0", TargetNamespace);
			return ns;
		}

		#endregion

		internal class ServiceDescriptionSerializer : XmlSerializer 
		{
			static XmlTypeMapping _typeMap;
			static Type[] _builtinExtensionTypes = new Type[] {
				typeof (HttpAddressBinding), 
				typeof (HttpBinding),
				typeof (HttpOperationBinding),
				typeof (HttpUrlEncodedBinding),
				typeof (HttpUrlReplacementBinding),
				typeof (MimeContentBinding),
				typeof (MimeMultipartRelatedBinding),
				typeof (MimePart),
				typeof (MimeTextBinding),
				typeof (MimeXmlBinding),
				typeof (SoapAddressBinding),
				typeof (SoapBinding),
				typeof (SoapBodyBinding),
				typeof (SoapFaultBinding),
				typeof (SoapHeaderBinding),
				typeof (SoapHeaderFaultBinding),
				typeof (SoapOperationBinding)
			};
			
			protected override void Serialize (object o, XmlSerializationWriter writer)
			{
				ServiceDescriptionWriter xsWriter = writer as ServiceDescriptionWriter;
				xsWriter.WriteObject (o);
			}
			
			protected override object Deserialize (XmlSerializationReader reader)
			{
				ServiceDescriptionReader xsReader = reader as ServiceDescriptionReader;
				return xsReader.ReadObject ();
			}
			
			protected override XmlSerializationWriter CreateWriter ()
			{
				return new ServiceDescriptionWriter (GetTypeMapping());
			}
			
			protected override XmlSerializationReader CreateReader ()
			{
				return new ServiceDescriptionReader (GetTypeMapping());
			}
			
			XmlTypeMapping GetTypeMapping ()
			{
				if (_typeMap == null) {
					XmlReflectionImporter ri = new XmlReflectionImporter (ServiceDescription.Namespace);
					foreach (Type t in _builtinExtensionTypes) ri.IncludeType (t);
					_typeMap = ri.ImportTypeMapping (typeof (ServiceDescription));
				}
				return _typeMap;
			}
		}
		
		internal class ServiceDescriptionWriter : XmlSerializationWriterInterpreter
		{
			public ServiceDescriptionWriter (XmlMapping typeMap)
			: base (typeMap)
			{
			}

			protected override void WriteObjectElementElements (XmlTypeMapping typeMap, object ob)
			{
				Type type = ob.GetType ();
				object[] ats = type.GetCustomAttributes (typeof(XmlFormatExtensionPointAttribute), true);
				if (ats.Length > 0)
				{
					XmlFormatExtensionPointAttribute at = (XmlFormatExtensionPointAttribute)ats[0];
					IEnumerable extensions = null;
					
					PropertyInfo prop = type.GetProperty (at.MemberName);
					if (prop != null)
						extensions = (IEnumerable) prop.GetValue (ob, null);
					else {
						FieldInfo field = type.GetField (at.MemberName);
						if (field != null)
							extensions = (IEnumerable) field.GetValue (ob);
						else
							throw new InvalidOperationException ("XmlFormatExtensionPointAttribute: Member " + at.MemberName + " not found");
					}
					
					if (extensions != null)
					{
						foreach (ServiceDescriptionFormatExtension ext in extensions)
							WriteExtension (ext);
					}
				}
				
				base.WriteObjectElementElements (typeMap, ob);
			}
			
			void WriteExtension (ServiceDescriptionFormatExtension ext)
			{
				string prefix = null;
				string ns = null;
				string name = null;
				
				Type type = ext.GetType ();
				
				object[] ats = type.GetCustomAttributes (typeof(XmlFormatExtensionPrefixAttribute), true);
				if (ats.Length > 0)
				{
					XmlFormatExtensionPrefixAttribute at = (XmlFormatExtensionPrefixAttribute)ats[0];
					prefix = at.Prefix;
					ns = at.Namespace;
				}
				
				ats = type.GetCustomAttributes (typeof(XmlFormatExtensionAttribute), true);
				if (ats.Length > 0)
				{
					XmlFormatExtensionAttribute at = (XmlFormatExtensionAttribute)ats[0];
					name = at.ElementName;
					if (at.Namespace != null) ns = at.Namespace;
				}
				
				if (name == null) throw new InvalidOperationException ("XmlFormatExtensionAttribute must be applied to type " + type);
				
				if (prefix == null || prefix == "") prefix = Writer.LookupPrefix (ns);
				
				if (prefix != null && prefix != "")
					Writer.WriteStartElement (prefix, name, ns);
				else
					WriteStartElement (name, ns, false);

				WriteObjectElement (GetTypeMap (type), ext, name, ns);
					
				WriteEndElement ();
			}
		}

		internal class ServiceDescriptionReader : XmlSerializationReaderInterpreter
		{
			public ServiceDescriptionReader (XmlMapping typeMap)
			: base (typeMap)
			{
			}
		}
	}
}
