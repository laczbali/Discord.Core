using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Core.Utils.ED25519
{
	internal static class Ed25519
	{
		/// <summary>
		/// internal Keys are 32 byte values. All possible values of this size a valid.
		/// </summary>
		internal const int internalKeySize = 32;
		/// <summary>
		/// Signatures are 64 byte values
		/// </summary>
		internal const int SignatureSize = 64;
		/// <summary>
		/// Private key seeds are 32 byte arbitrary values. This is the form that should be generated and stored.
		/// </summary>
		internal const int PrivateKeySeedSize = 32;
		/// <summary>
		/// A 64 byte expanded form of private key. This form is used internally to improve performance
		/// </summary>
		internal const int ExpandedPrivateKeySize = 32 * 2;

		/// <summary>
		/// Verify Ed25519 signature
		/// </summary>
		/// <param name="signature">Signature bytes</param>
		/// <param name="message">Message</param>
		/// <param name="internalKey">internal key</param>
		/// <returns>True if signature is valid, false if it's not</returns>
		internal static bool Verify(ArraySegment<byte> signature, ArraySegment<byte> message, ArraySegment<byte> internalKey)
		{
			if (signature.Count != SignatureSize)
				throw new ArgumentException($"Sizeof signature doesnt match defined size of {SignatureSize}");

			if (internalKey.Count != internalKeySize)
				throw new ArgumentException($"Sizeof internal key doesnt match defined size of {internalKeySize}");

			if(signature.Array == null)
				throw new ArgumentNullException(nameof(signature));

			if (message.Array == null)
				throw new ArgumentNullException(nameof(message));

			if (internalKey.Array == null)
				throw new ArgumentNullException(nameof(internalKey));

			return Ed25519Operations.crypto_sign_verify(signature.Array, signature.Offset, message.Array, message.Offset, message.Count, internalKey.Array, internalKey.Offset);
		}

		/// <summary>
		/// Verify Ed25519 signature
		/// </summary>
		/// <param name="signature">Signature bytes</param>
		/// <param name="message">Message</param>
		/// <param name="internalKey">internal key</param>
		/// <returns>True if signature is valid, false if it's not</returns>
		internal static bool Verify(byte[] signature, byte[] message, byte[] internalKey)
		{
			if (signature == null) throw new ArgumentNullException(nameof(signature));
			if (message == null) throw new ArgumentNullException(nameof(message));
			if (internalKey == null) throw new ArgumentNullException(nameof(internalKey));
			if (signature.Length != SignatureSize)
				throw new ArgumentException($"Sizeof signature doesnt match defined size of {SignatureSize}");

			if (internalKey.Length != internalKeySize)
				throw new ArgumentException($"Sizeof internal key doesnt match defined size of {internalKeySize}");

			return Ed25519Operations.crypto_sign_verify(signature, 0, message, 0, message.Length, internalKey, 0);
		}
	}
}
