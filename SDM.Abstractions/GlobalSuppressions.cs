// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Minor Code Smell", "S4027:Exceptions should provide standard constructors", Justification = "It should be instantiated using the builder", Scope = "type", Target = "~T:Skyline.DataMiner.SDM.SdmBulkCrudException`1")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:Field names should not use Hungarian notation", Justification = "Incorrect Warning", Scope = "member", Target = "~M:Skyline.DataMiner.SDM.UniversalComparer.Compare``1(``0,``0)~System.Int32")]
[assembly: SuppressMessage("Major Code Smell", "S3881:\"IDisposable\" should be implemented correctly", Justification = "It's fine", Scope = "type", Target = "~T:Skyline.DataMiner.SDM.Middleware.MiddlewareObservableRepository`1")]
